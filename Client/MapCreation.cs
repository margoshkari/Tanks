﻿

namespace Client
{
    public static class MapCreation
    {
        public static char[,] map = new char[12, 20];
        static MapCreation()
        {

            map = new char[12, 20]{
                    {'X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X'},
                    {'X',' ',' ',' ',' ','X',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ','X',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ','X',' ',' ',' ','X','X',' ',' ',' ',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ','X','X',' ',' ',' ','X','X',' ',' ',' ',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ','X','X',' ',' ',' ',' ','X','X',' ',' ',' ',' ',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ',' ',' ',' ',' ','X','X',' ',' ',' ',' ','X','X',' ',' ','X'},
                    {'X',' ',' ',' ',' ',' ',' ',' ',' ','X','X',' ',' ',' ','X','X',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ',' ',' ',' ',' ','X','X',' ',' ',' ','X',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','X',' ',' ',' ',' ','X'},
                    {'X',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','X',' ',' ',' ',' ','X'},
                    {'X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X'}
                };
        }
    }
}
