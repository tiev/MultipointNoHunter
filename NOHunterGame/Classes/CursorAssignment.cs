using System.Drawing;

namespace NOHunterGame.Classes
{
    /// <summary>
    /// Provides a link between MouseID and Mouse Cursors to ensure mice 
    ///  maintain their same cursors when other mice attached or removed
    /// </summary>
    public class CursorAssignment
    {
        public Bitmap Cursor;
        public int MouseID;

        public CursorAssignment(Bitmap cursor, int mouseID)
        {
            this.Cursor = cursor;
            this.MouseID = mouseID;
        }
    }

    /// <summary>
    /// Contains a collection of CursorAssignment objects
    /// </summary>
    public sealed class CursorAssignments
    {
        private static readonly CursorAssignments instance = new CursorAssignments();
  
        #region Initialise Cursor Assignments 

        /// <summary>
        ///  Array of cursorAssignment objects to keep track of cursors and the mice to which they're assigned
        ///  Assign MouseID of -1 initially to indicate that there is no Cursor currently unassigned
        /// </summary>
        private CursorAssignment[] assignments = new CursorAssignment[]
        { 
            new CursorAssignment(Properties.Resources.elephant, -1),
            new CursorAssignment(Properties.Resources.tiger, -1),
            new CursorAssignment(Properties.Resources.hippo, -1),
            new CursorAssignment(Properties.Resources.lion, -1), 
            new CursorAssignment(Properties.Resources.rooster2, -1),
            new CursorAssignment(Properties.Resources.boy_01, -1),
            new CursorAssignment(Properties.Resources.girl_03, -1),
            new CursorAssignment(Properties.Resources.boy_02, -1),
            new CursorAssignment(Properties.Resources.boy_03, -1),
            new CursorAssignment(Properties.Resources.boy_04, -1),
            new CursorAssignment(Properties.Resources.boy_05, -1),
            new CursorAssignment(Properties.Resources.boy_06, -1),
            new CursorAssignment(Properties.Resources.boy_07, -1),
            new CursorAssignment(Properties.Resources.boy_08, -1),
            new CursorAssignment(Properties.Resources.cat, -1), 
            new CursorAssignment(Properties.Resources.cow, -1),
            new CursorAssignment(Properties.Resources.dog, -1), 
            new CursorAssignment(Properties.Resources.donkey, -1),
            new CursorAssignment(Properties.Resources.giraffe, -1),
            new CursorAssignment(Properties.Resources.girl_01, -1),
            new CursorAssignment(Properties.Resources.girl_02, -1),
            new CursorAssignment(Properties.Resources.girl_04, -1),
            new CursorAssignment(Properties.Resources.girl_05, -1),
            new CursorAssignment(Properties.Resources.girl_06, -1),
            new CursorAssignment(Properties.Resources.girl_07, -1),
            new CursorAssignment(Properties.Resources.horse, -1),
            new CursorAssignment(Properties.Resources.monkey, -1),
            new CursorAssignment(Properties.Resources.ox, -1),
            new CursorAssignment(Properties.Resources.pig, -1),
            new CursorAssignment(Properties.Resources.rabbit, -1),
            new CursorAssignment(Properties.Resources.rihino, -1),
            new CursorAssignment(Properties.Resources.rooster, -1)
        };

        public static CursorAssignments Instance 
        {
            get
            {
                return instance;
            }
        }
        
        public CursorAssignment[] Assignments
        {
            get { return this.assignments; }
        }

        #endregion Initialise Cursor Assignments
    }
}
