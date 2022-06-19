

namespace Démineur
{
    public struct Box
    {
        private bool is_bomb;
        public enum State { None, Flag, Discovered };
        private State status;
        private short cursor_line;
        private short cursor_row;
        private short mines_around;
        public bool Is_bomb
        {
            get { return is_bomb; }
            set { is_bomb = value; }
        }
        public State Status
        {
            get { return status; }
            set { status = value; }
        }
        public short Cursor_line
        {
            get { return cursor_line; }
            set { cursor_line = value; }
        }
        public short Cursor_row
        {
            get { return cursor_row; }
            set { cursor_row = value; }
        }
        public short Mines_around
        {
            get { return mines_around; }
            set { mines_around = value; }
        }
    }
}
