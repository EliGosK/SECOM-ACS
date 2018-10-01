namespace CSI.ComponentModel.Design
{
    using CSI.Resources;
    using System;
    using System.ComponentModel;
    using System.Resources;
    using System.Runtime.CompilerServices;

    public class ResourceDescriptionAttribute : DescriptionAttribute
    {
        private bool resourceLoaded;

        public ResourceDescriptionAttribute()
        {
        }

        public ResourceDescriptionAttribute(Type resourceType, string resourceName)
        {
            this.ResourceType = resourceType;
            this.ResourceName = resourceName;
        }

        private void EnsureDescriptionLoaded()
        {
            if (!this.resourceLoaded)
            {
                ResourceManager manager = new ResourceManager(this.ResourceType);
                try
                {
                    base.DescriptionValue = manager.GetString(this.ResourceName);
                }
                catch (MissingManifestResourceException)
                {
                    base.DescriptionValue = this.ResourceName;
                }
                base.DescriptionValue = (base.DescriptionValue == null) ? string.Empty : base.DescriptionValue;
                this.resourceLoaded = true;
            }
        }

        public override string Description
        {
            get
            {
                this.EnsureDescriptionLoaded();
                return base.DescriptionValue;
            }
        }

        public string ResourceName { get; set; }

        public Type ResourceType { get; set; }
    }
}

