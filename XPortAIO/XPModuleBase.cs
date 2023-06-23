using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPortAIO
{
    internal class XPModuleBase
    {
        internal bool IsChampionModule;

        /// <summary>
        /// Initializes the champion module.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Terminates the champion module.
        /// </summary>
        public virtual void Terminate()
        {
        }
    }
}
