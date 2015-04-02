using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.MultiPoint.MultiPointCommonTypes;
using Microsoft.MultiPoint.MultiPointMousePlugIn;
using Microsoft.MultiPoint.MultiPointSDK;

namespace NOHunterGame.Classes
{
    /// <summary>
    /// A helper class that contains various functions related to MultiPoint functionality
    /// </summary>
    public static class MouseHelper
    {
        /// <summary>
        /// Helper method that will create a Bitmap image out of a Bitmap 
        /// </summary>
        /// <param name="b">The bitmap image that has to be processed</param>
        /// <returns>The bitmap image created from the bitmap parameter</returns>
        public static BitmapImage CreateBitmapImage(Bitmap b)
        {
            BitmapImage bmpimg = new BitmapImage();
            MemoryStream memStream = new MemoryStream();
            bmpimg.BeginInit();
            b.MakeTransparent(Color.White);
            b.Save(memStream, ImageFormat.Png);
            bmpimg.StreamSource = memStream;
            bmpimg.EndInit();
            return bmpimg;
        }

        /// <summary>
        /// Check if there are too many mice, if so disable extras
        /// </summary>
        public static bool CheckIfTooManyMice()
        {
            // Too many mice - disable extras
            if (MultiPointSDK.GetInstance().MouseDeviceList.Count > CursorAssignments.Instance.Assignments.Length)
            {
                // Disable extra mice.  
                for (int i = CursorAssignments.Instance.Assignments.Length; i < MultiPointSDK.GetInstance().MouseDeviceList.Count - 1; i++)
                    ((MultiPointMouseDevice)((DeviceInfo)MultiPointSDK.GetInstance().MouseDeviceList[i]).DeviceVisual).Visible = false;

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Assign cursor to mouse object
        /// </summary>
        /// <param name="mouseObject">The mouse to assign a cursor to</param>
        public static void AssignCursorToMouse(DeviceInfo mouseObject)
        {
            // Get available cursors
            var available = (from a in CursorAssignments.Instance.Assignments
                             where a.MouseID == -1
                             select a).ToArray();

            // Assign a random cursors from available
            var assignment = available[(new Random()).Next(available.Length)];
            var mouseDevice = (MultiPointMouseDevice)mouseObject.DeviceVisual;
            mouseDevice.CursorImage = CreateBitmapImage(assignment.Cursor);
            assignment.MouseID = int.Parse(mouseObject.DeviceID);
        }

        /// <summary>
        /// Remove cursor from mouse object
        /// </summary>
        /// <param name="mouseObject">The mouse to remove the cursor from</param>
        public static void RemoveCursorFromMouse(DeviceInfo mouseObject)
        {
            int mouseID = int.Parse(mouseObject.DeviceID);
            var assignment = CursorAssignments.Instance.Assignments.FirstOrDefault(ca => ca.MouseID == mouseID);
            if (assignment != null)
                assignment.MouseID = -1;
        }
    }
}
