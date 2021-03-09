namespace ResearchProgram
{
    public class Customer : IContainer
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ShortTitle { get; set; }

        public Customer()
        {
            Title = "";
        }

        public string GetTitle()
        {
            return Title;
        }

        public override string ToString()
        {
            if (ShortTitle != "")
                return ShortTitle;
            else
                return Title;
        }
    }
}
