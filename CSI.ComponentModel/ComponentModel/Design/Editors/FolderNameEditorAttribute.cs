namespace CSI.ComponentModel.Design.Editors
{
    using CSI.ComponentModel;
    using Resources;
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [AttributeUsage(AttributeTargets.Property)]
    public class FolderNameEditorAttribute : Attribute
    {
        public FolderNameEditorAttribute()
        {
            this.Description = "Choose a folder.";
            this.RootFolder = Environment.SpecialFolder.MyComputer;
            this.ShowNewFolderButton = true;
        }

        public FolderNameEditorAttribute(string descripiton)
        {
            this.Description = descripiton;
        }

        public FolderNameEditorAttribute(System.Type resourceType, string resourceKey)
        {
            this.Description = StringResourceHelper.GetString(resourceType, resourceKey);
        }

        public FolderBrowserDialog CreateFolderBrowserDialog()
        {
            return new FolderBrowserDialog { Description = this.Description, RootFolder = this.RootFolder, ShowNewFolderButton = this.ShowNewFolderButton };
        }

        public string Description { get; set; }

        public Environment.SpecialFolder RootFolder { get; set; }

        public bool ShowNewFolderButton { get; set; }
    }
}

