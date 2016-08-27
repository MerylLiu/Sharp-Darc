namespace Darc.Dapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class SequenceAttribute : BaseAttribute
    {
        public string Sequence { get; set; }

        public SequenceAttribute(string sequence)
        {
            Sequence = sequence;
        }
    }
}