namespace MyDBTestApp
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return $"{ Firstname } { LastName }";
            }
        }
    }
}