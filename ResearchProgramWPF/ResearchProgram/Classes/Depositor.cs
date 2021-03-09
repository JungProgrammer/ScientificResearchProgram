namespace ResearchProgram
{
    public class Depositor : IContainer
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
