namespace PlagiarismChecker.Models
{
    public class StudentResult
    {

        public string firstDoc { get; set; }
        public string secondDoc { get; set; }
        public decimal Score { get; set; }
        public StudentResult(string firstDoc, string secondDoc, decimal score)
        {
            this.firstDoc = firstDoc;
            this.secondDoc = secondDoc;
            this.Score = score;
        }
        public StudentResult()
        {

        }

        public bool Equals(StudentResult CompareWith)
        {
            if (firstDoc == CompareWith.firstDoc && secondDoc == CompareWith.secondDoc && Score == CompareWith.Score)
            {
                return true;
            }
            return false;
        }
        public static bool Equals(StudentResult r1, StudentResult r2)
        {
            if(r1.firstDoc == r2.firstDoc && r1.secondDoc == r2.secondDoc && r1.Score == r2.Score)
            {
                return true;
            }
            return false;
        }
    }
}
