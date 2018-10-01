namespace CSI.ComponentModel.Design.Editors
{
    using CSI.Utilities;
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Windows.Forms;

    public class FolderNameEditor : UITypeEditor
    {
        private FolderBrowserDialog dialog;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (this.dialog == null)
            {
                FolderNameEditorAttribute attribute = AttributeHelper.GetAttribute<FolderNameEditorAttribute>(context.GetType(), true);
                if (attribute != null)
                {
                    this.dialog = attribute.CreateFolderBrowserDialog();
                }
                if (this.dialog == null)
                {
                    this.dialog = new FolderBrowserDialog();
                    this.InitializeDialog(this.dialog);
                }
            }
            string currentDirectory = (string) value;
            if (string.IsNullOrEmpty(currentDirectory))
            {
                currentDirectory = Environment.CurrentDirectory;
            }
            this.dialog.SelectedPath = currentDirectory;
            if (this.dialog.ShowDialog() != DialogResult.OK)
            {
                return value;
            }
            return this.dialog.SelectedPath;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public virtual void InitializeDialog(FolderBrowserDialog dialog)
        {
        }
    }
}

