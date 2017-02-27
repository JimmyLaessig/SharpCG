using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;


namespace SharpCG.Core
{

    public interface IWindow
    {
        IMouse Mouse
        {
            get;
        }
        IKeyboard Keyboard
        {
            get;
        }

        Vector2 ScreenSize
        {
            get;
        }

        Vector2 Position
        {
            get;
            set;
        }

        void registerResizeCallback();
    }
}

