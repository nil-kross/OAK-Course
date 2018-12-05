using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Components
{
    public class CustomComponent : Component
    {
        private String fileNameString = null;
        private String folderNameString = null;

        public override String FileName
        {
            get { return this.fileNameString; }
        }
        protected override String FolderPathwayString
        {
            get { return this.folderNameString; }
        }

        public CustomComponent(String fileName, String folder)
        {
            this.fileNameString = fileName;
            this.folderNameString = folder;
        }
    }
}
