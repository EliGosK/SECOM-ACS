namespace CSI.Resources
{
    using System;
    using System.Resources;

    public static class StringResourceHelper
    {
        public static string GetString(Type resourceType, string name, params object[] args)
        {
            string format = new ResourceManager(resourceType).GetString(name);
            if (format == null)
            {
                return null;
            }
            if ((args != null) && (args.Length > 0))
            {
                return string.Format(format, args);
            }
            return format;
        }
    }
}

