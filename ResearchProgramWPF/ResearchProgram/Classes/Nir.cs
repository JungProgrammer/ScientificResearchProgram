﻿namespace ResearchProgram
{
    public class Nir : IContainer
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string GetTitle()
        {
            return Title;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
