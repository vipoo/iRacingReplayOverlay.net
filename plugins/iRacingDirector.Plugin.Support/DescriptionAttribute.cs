using System;

namespace iRacingDirector.Plugin
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public DescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}