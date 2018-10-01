namespace CSI.ComponentModel.Design
{
    using System;
    using System.ComponentModel;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ResourceDisplayNameAttribute : DisplayNameAttribute
    {
        private bool resourceLoaded;

        public ResourceDisplayNameAttribute()
        {
        }

        public ResourceDisplayNameAttribute(Type resourceType, string resourceName)
        {
            this.ResourceType = resourceType;
            this.ResourceName = resourceName;
        }

        private void EnsureDisplayNameLoaded()
        {
            if (!this.resourceLoaded)
            {
                ResourceManager manager = new ResourceManager(this.ResourceType);
                try
                {
                    base.DisplayNameValue = manager.GetString(this.ResourceName);
                }
                catch (MissingManifestResourceException)
                {
                    base.DisplayNameValue = this.ResourceName;
                }
                if (string.IsNullOrEmpty(base.DisplayNameValue))
                {
                    base.DisplayNameValue = this.ResourceName;
                }
                this.resourceLoaded = true;
            }
        }

        public override string DisplayName
        {
            get
            {
                this.EnsureDisplayNameLoaded();
                return base.DisplayNameValue;
            }
        }

        public string ResourceName { get; set; }

        public Type ResourceType { get; set; }
    }
}

