namespace JWT_with_Student_details.Model
{
    public class JWTStudentDetails
    {
        public int StudentID { get; set; }
        public required string Name { get; set; }
        public int Age { get; set; }
        public required string Email { get; set; }
    }
}
