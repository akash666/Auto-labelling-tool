using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvisLabellingAutomationtoolVersion1._2
{
    class LabelClass1
    {
        public string labelName;
        public string get_labelName
        {
            get
            {
                return labelName;
            }
            set
            {
                labelName = value;
            }
        }

        public double posXMin;
        public double getPos_XMin
        {
            get
            {
                return posXMin;
            }
            set
            {
                posXMin = value;
            }
        }

        public double posYMin;
        public double getPos_YMin
        {
            get
            {
                return posYMin;
            }
            set
            {
                posYMin = value;
            }
        }
        public double posXMax;
        public double getPos_XMAx
        {
            get
            {
                return posXMax;
            }
            set
            {
                posXMax = value;
            }
        }

        public double posYMax;
        public double getPos_YMAx
        {
            get
            {
                return posYMax;
            }
            set
            {
                posYMax = value;
            }
        }

        public double width;
        public double setWidth
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public double height;
        public double setHeight
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }
        public override string ToString()
        {
            return this.labelName;
        }
    }
}
