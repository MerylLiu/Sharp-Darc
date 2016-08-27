namespace Darc.Dapper.Expressions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Text.RegularExpressions;
    using global::Dapper;

    public class QueryTranslator : ExpressionVisitor
    {
        private StringBuilder _sb;
        public DynamicParameters Parameters = new DynamicParameters();

        public string Translate(Expression expression)
        {
            _sb = new StringBuilder();
            Visit(expression);
            return _sb.ToString();
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression) e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof (QueryExtension) && m.Method.Name == "WhereC")
            {
                var lambda = (LambdaExpression) StripQuotes(m.Arguments[1]);
                Visit(lambda.Body);
                return m;
            }

            if (m.Method.Name == "Contains")
            {
                _sb.Append($"AND {m.Object} Like %{m.Arguments[0].ToString().Replace("\"", string.Empty)}% ");
                return m;
            }

            if (m.Method.Name == "StartsWith")
            {
                _sb.Append($"AND {m.Object} Like {m.Arguments[0].ToString().Replace("\"", string.Empty)}% ");
                return m;
            }

            if (m.Method.Name == "EndsWith")
            {
                _sb.Append($"AND {m.Object} Like '{m.Arguments[0].ToString().Replace("\"", string.Empty)}%' ");
                return m;
            }

            return base.VisitMethodCall(m);
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    _sb.Append(" NOT ");
                    Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException($"Operational character '{u.NodeType}' is not supported.");
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    _sb.Append(" AND ");
                    break;
                case ExpressionType.AndAlso:
                    _sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    _sb.Append(" OR");
                    break;
                case ExpressionType.Equal:
                    _sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    _sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    _sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    _sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _sb.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException($"Operational character '{b.NodeType}' is not supported.");
            }
            Visit(b.Right);
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            var q = c.Value as IQueryable;
            if (q != null)
            {
                _sb.Append(q.ElementType.Name);
            }
            else if (c.Value == null)
            {
                _sb.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        _sb.Append(((bool) c.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        var rex = new Regex(@"^\d+$");
                        if (rex.IsMatch(c.Value.ToString()))
                        {
                            _sb.Append($"{c.Value}");
                        }
                        else
                        {
                            _sb.Append($"'{c.Value}'");
                        }

                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException($"Constant '{c.Value}' is not supported.");
                    default:
                        _sb.Append(c.Value);
                        break;
                }
            }
            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                _sb.Append(m.Member.Name);
                return m;
            }

            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Constant)
            {
                _sb.Append(((ConstantExpression) m.Expression).Value);
                return m;
            }

            throw new NotSupportedException($"Member '{m.Member.Name}' is not supported.");
        }
    }
}