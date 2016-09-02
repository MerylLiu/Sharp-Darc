namespace Dapper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using Oracle.ManagedDataAccess.Client;

    public class OracleDynamicParameters : SqlMapper.IDynamicParameters, SqlMapper.IParameterLookup,
        SqlMapper.IParameterCallbacks
    {
        private static readonly Dictionary<SqlMapper.Identity, Action<IDbCommand, object>> ParamReaderCache =
            new Dictionary<SqlMapper.Identity, Action<IDbCommand, object>>();

        private readonly Dictionary<string, Action<object, OracleDynamicParameters>> _cachedOutputSetters =
            new Dictionary<string, Action<object, OracleDynamicParameters>>();

        private readonly Dictionary<string, ParamInfo> _parameters = new Dictionary<string, ParamInfo>();
        private List<Action> _outputCallbacks;
        private List<object> _templates;

        /// <summary>
        ///     construct a dynamic parameter bag
        /// </summary>
        public OracleDynamicParameters()
        {
            RemoveUnused = true;
        }

        /// <summary>
        ///     construct a dynamic parameter bag
        /// </summary>
        /// <param name="template">can be an anonymous type or a OracleDynamicParameters bag</param>
        public OracleDynamicParameters(object template)
        {
            RemoveUnused = true;
            AddDynamicParams(template);
        }

        /// <summary>
        ///     If true, the command-text is inspected and only values that are clearly used are included on the connection
        /// </summary>
        public bool RemoveUnused { get; set; }

        /// <summary>
        ///     All the names of the param in the bag, use Get to yank them out
        /// </summary>
        public IEnumerable<string> ParameterNames
        {
            get { return _parameters.Select(p => p.Key); }
        }

        void SqlMapper.IDynamicParameters.AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            AddParameters(command, identity);
        }

        void SqlMapper.IParameterCallbacks.OnCompleted()
        {
            foreach (var param in (from p in _parameters select p.Value))
            {
                if (param.OutputCallback != null) param.OutputCallback(param.OutputTarget, this);
            }
        }

        object SqlMapper.IParameterLookup.this[string member]
        {
            get
            {
                ParamInfo param;
                return _parameters.TryGetValue(member, out param) ? param.Value : null;
            }
        }

        /// <summary>
        ///     Append a whole object full of params to the dynamic
        ///     EG: AddDynamicParams(new {A = 1, B = 2}) // will add property A and B to the dynamic
        /// </summary>
        /// <param name="param"></param>
        public void AddDynamicParams(object param)
        {
            var obj = param;
            if (obj != null)
            {
                var subDynamic = obj as OracleDynamicParameters;
                if (subDynamic == null)
                {
                    var dictionary = obj as IEnumerable<KeyValuePair<string, object>>;
                    if (dictionary == null)
                    {
                        _templates = _templates ?? new List<object>();
                        _templates.Add(obj);
                    }
                    else
                    {
                        foreach (var kvp in dictionary)
                        {
                            Add(kvp.Key, kvp.Value, null, null, null);
                        }
                    }
                }
                else
                {
                    if (subDynamic._parameters != null)
                    {
                        foreach (var kvp in subDynamic._parameters)
                        {
                            _parameters.Add(kvp.Key, kvp.Value);
                        }
                    }

                    if (subDynamic._templates != null)
                    {
                        _templates = _templates ?? new List<object>();
                        foreach (var t in subDynamic._templates)
                        {
                            _templates.Add(t);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Add a parameter to this dynamic parameter list
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        public void Add(
#if CSHARP30
string name, object value, DbType? dbType, ParameterDirection? direction, int? size
#else
            string name, object value = null, OracleDbType? dbType = null, ParameterDirection? direction = null,
            int? size = null
#endif
            )
        {
            _parameters[Clean(name)] = new ParamInfo
            {
                Name = name,
                Value = value,
                ParameterDirection = direction ?? ParameterDirection.Input,
                DbType = dbType,
                Size = size
            };
        }

        private static string Clean(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                switch (name[0])
                {
                    case '@':
                    case ':':
                    case '?':
                        return name.Substring(1);
                }
            }
            return name;
        }

        /// <summary>
        ///     Add all the parameters needed to the command just before it executes
        /// </summary>
        /// <param name="command">The raw command prior to execution</param>
        /// <param name="identity">Information about the query</param>
        protected void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            var literals = SqlMapper.GetLiteralTokens(identity.sql);

            if (_templates != null)
            {
                foreach (var template in _templates)
                {
                    var newIdent = identity.ForDynamicParameters(template.GetType());
                    Action<IDbCommand, object> appender;

                    lock (ParamReaderCache)
                    {
                        if (!ParamReaderCache.TryGetValue(newIdent, out appender))
                        {
                            appender = SqlMapper.CreateParamInfoGenerator(newIdent, true, RemoveUnused, literals);
                            ParamReaderCache[newIdent] = appender;
                        }
                    }

                    appender(command, template);
                }

                // Now that the parameters are added to the command, let's place our output callbacks
                var tmp = _outputCallbacks;
                if (tmp != null)
                {
                    foreach (var generator in tmp)
                    {
                        generator();
                    }
                }
            }

            foreach (var param in _parameters.Values)
            {
                var name = Clean(param.Name);

                var add = !((OracleCommand) command).Parameters.Contains(name);
                OracleParameter p;
                if (add)
                {
                    p = ((OracleCommand) command).CreateParameter();
                    p.ParameterName = name;
                }
                else
                {
                    p = ((OracleCommand) command).Parameters[name];
                }
                var val = param.Value;

                var isCustomQueryParameter = val is SqlMapper.ICustomQueryParameter;
                if (param.Value is IList)
                {
#pragma warning disable 612, 618
                    SqlMapper.PackListParameters(command, name, val);
#pragma warning restore 612, 618
                }
                else if (isCustomQueryParameter)
                {
                    ((SqlMapper.ICustomQueryParameter) val).AddParameter(command, name);
                }
                else
                {
                    p.Value = val ?? DBNull.Value;
                    p.Direction = param.ParameterDirection;
                    var s = val as string;
                    if (s != null)
                    {
                        if (s.Length <= 4000)
                        {
                            p.Size = 4000;
                        }
                    }
                    if (param.Size != null)
                    {
                        p.Size = param.Size.Value;
                    }
                    if (param.DbType != null)
                    {
                        p.OracleDbType = param.DbType.Value;
                    }
                    if (add)
                    {
                        command.Parameters.Add(p);
                    }
                    param.AttachedParam = p;
                }
            }
        }

        /// <summary>
        ///     Get the value of a parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns>The value, note DBNull.Value is not returned, instead the value is returned as null</returns>
        public T Get<T>(string name)
        {
            var val = _parameters[Clean(name)].AttachedParam.Value;
            if (val == DBNull.Value)
            {
                if (default(T) != null)
                {
                    throw new ApplicationException("Attempting to cast a DBNull to a non nullable type!");
                }
                return default(T);
            }
            return (T) val;
        }

        /// <summary>
        ///     Allows you to automatically populate a target property/field from output parameters. It actually
        ///     creates an InputOutput parameter, so you can still pass data in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The object whose property/field you wish to populate.</param>
        /// <param name="expression">A MemberExpression targeting a property/field of the target (or descendant thereof.)</param>
        /// <param name="dbType"></param>
        /// <param name="size">The size to set on the parameter. Defaults to 0, or DbString.DefaultLength in case of strings.</param>
        /// <returns>The OracleDynamicParameters instance</returns>
#if CSHARP30
        public OracleDynamicParameters Output<T>(T target, Expression<Func<T, object>> expression, DbType? dbType, int? size)
#else
        public OracleDynamicParameters Output<T>(T target, Expression<Func<T, object>> expression, DbType? dbType = null,
            int? size = null)
#endif
        {
            var failMessage = "Expression must be a property/field chain off of a(n) {0} instance";
            failMessage = string.Format(failMessage, typeof (T).Name);
            Action @throw = () => { throw new InvalidOperationException(failMessage); };

            // Is it even a MemberExpression?
            var lastMemberAccess = expression.Body as MemberExpression;

            if (lastMemberAccess == null ||
                (lastMemberAccess.Member.MemberType != MemberTypes.Property &&
                 lastMemberAccess.Member.MemberType != MemberTypes.Field))
            {
                if (expression.Body.NodeType == ExpressionType.Convert &&
                    expression.Body.Type == typeof (object) &&
                    ((UnaryExpression) expression.Body).Operand is MemberExpression)
                {
                    // It's got to be unboxed
                    lastMemberAccess = (MemberExpression) ((UnaryExpression) expression.Body).Operand;
                }
                else @throw();
            }

            // Does the chain consist of MemberExpressions leading to a ParameterExpression of type T?
            var diving = lastMemberAccess;
            ParameterExpression constant = null;
            // Retain a list of member names and the member expressions so we can rebuild the chain.
            var names = new List<string>();
            var chain = new List<MemberExpression>();

            do
            {
                // Insert the names in the right order so expression 
                // "Post.Author.Name" becomes parameter "PostAuthorName"
                names.Insert(0, diving.Member.Name);
                chain.Insert(0, diving);

                constant = diving.Expression as ParameterExpression;
                diving = diving.Expression as MemberExpression;

                if (constant != null &&
                    constant.Type == typeof (T))
                {
                    break;
                }
                if (diving == null ||
                    (diving.Member.MemberType != MemberTypes.Property &&
                     diving.Member.MemberType != MemberTypes.Field))
                {
                    @throw();
                }
            } while (diving != null);

            var dynamicParamName = string.Join(string.Empty, names.ToArray());

            // Before we get all emitty...
            var lookup = string.Join("|", names.ToArray());

            var cache = CachedOutputSetters<T>.Cache;
            var setter = (Action<object, OracleDynamicParameters>) cache[lookup];

            if (setter != null) goto MAKECALLBACK;

            // Come on let's build a method, let's build it, let's build it now!
            var dm = new DynamicMethod(string.Format("ExpressionParam{0}", Guid.NewGuid()), null,
                new[] {typeof (object), GetType()}, true);
            var il = dm.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // [object]
            il.Emit(OpCodes.Castclass, typeof (T)); // [T]

            // Count - 1 to skip the last member access
            var i = 0;
            for (; i < (chain.Count - 1); i++)
            {
                var member = chain[0].Member;

                if (member.MemberType == MemberTypes.Property)
                {
                    var get = ((PropertyInfo) member).GetGetMethod(true);
                    il.Emit(OpCodes.Callvirt, get); // [Member{i}]
                }
                else // Else it must be a field!
                {
                    il.Emit(OpCodes.Ldfld, ((FieldInfo) member)); // [Member{i}]
                }
            }

            var paramGetter =
                GetType().GetMethod("Get", new[] {typeof (string)}).MakeGenericMethod(lastMemberAccess.Type);

            il.Emit(OpCodes.Ldarg_1); // [target] [OracleDynamicParameters]
            il.Emit(OpCodes.Ldstr, dynamicParamName); // [target] [OracleDynamicParameters] [ParamName]
            il.Emit(OpCodes.Callvirt, paramGetter); // [target] [value], it's already typed thanks to generic method

            // GET READY
            var lastMember = lastMemberAccess.Member;
            if (lastMember.MemberType == MemberTypes.Property)
            {
                var set = ((PropertyInfo) lastMember).GetSetMethod(true);
                il.Emit(OpCodes.Callvirt, set); // SET
            }
            else
            {
                il.Emit(OpCodes.Stfld, ((FieldInfo) lastMember)); // SET
            }

            il.Emit(OpCodes.Ret); // GO

            setter =
                (Action<object, OracleDynamicParameters>)
                    dm.CreateDelegate(typeof (Action<object, OracleDynamicParameters>));
            lock (cache)
            {
                cache[lookup] = setter;
            }

            // Queue the preparation to be fired off when adding parameters to the DbCommand
            MAKECALLBACK:
            (_outputCallbacks ?? (_outputCallbacks = new List<Action>())).Add(() =>
            {
                // Finally, prep the parameter and attach the callback to it
                ParamInfo parameter;
                var targetMemberType = lastMemberAccess.Type;
                var sizeToSet = (!size.HasValue && targetMemberType == typeof (string))
                    ? DbString.DefaultLength
                    : size ?? 0;

                if (_parameters.TryGetValue(dynamicParamName, out parameter))
                {
                    parameter.ParameterDirection = parameter.AttachedParam.Direction = ParameterDirection.InputOutput;

                    if (parameter.AttachedParam.Size == 0)
                    {
                        parameter.Size = parameter.AttachedParam.Size = sizeToSet;
                    }
                }
                else
                {
                    SqlMapper.ITypeHandler handler;
                    dbType = (!dbType.HasValue)
                        ? SqlMapper.LookupDbType(targetMemberType, targetMemberType.Name, true, out handler)
                        : dbType;

                    // CameFromTemplate property would not apply here because this new param
                    // Still needs to be added to the command
                    Add(dynamicParamName, expression.Compile().Invoke(target), null, ParameterDirection.InputOutput,
                        sizeToSet);
                }

                parameter = _parameters[dynamicParamName];
                parameter.OutputCallback = setter;
                parameter.OutputTarget = target;
            });

            return this;
        }

        private class ParamInfo
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public ParameterDirection ParameterDirection { get; set; }
            public OracleDbType? DbType { get; set; }
            public int? Size { get; set; }
            public IDbDataParameter AttachedParam { get; set; }
            internal Action<object, OracleDynamicParameters> OutputCallback { get; set; }
            internal object OutputTarget { get; set; }
            internal bool CameFromTemplate { get; set; }
        }

        internal static class CachedOutputSetters<T>
        {
            public static readonly Hashtable Cache = new Hashtable();
        }
    }
}