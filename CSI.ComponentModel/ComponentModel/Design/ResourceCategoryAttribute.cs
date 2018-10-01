namespace CSI.ComponentModel.Design
{
    using CSI.ComponentModel;
    using Resources;
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class ResourceCategoryAttribute : CategoryAttribute
    {
        private static ResourceCategoryAttribute general = new ResourceCategoryAttribute(typeof(ResourceCategoryAttribute), "CategoryGeneral");
        private readonly Type resourceType;

        public ResourceCategoryAttribute(Type resourceType, string category) : base(category)
        {
            this.resourceType = resourceType;
        }

        protected override string GetLocalizedString(string value)
        {
            return StringResourceHelper.GetString(this.resourceType, value);
        }

        public static ResourceCategoryAttribute General
        {
            get
            {
                return general;
            }
        }

        public Type ResourceType
        {
            get
            {
                return this.resourceType;
            }
        }
    }
}

