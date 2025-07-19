public class ElderlyDto
{
    public string Name { get; set; }
    public string IdCardNumber { get; set; }
    public string Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public string ContactPhone { get; set; }
    public string Address { get; set; }
    public string EmergencyContact { get; set; }

    public HealthAssessment Assessment { get; set; }
    public HealthMonitoring Monitoring { get; set; }
    public Family Family { get; set; }
}

public class HealthAssessment
{
    public int PhysicalHealthFunction { get; set; }
    public int PsychologicalFunction { get; set; }
    public int CognitiveFunction { get; set; }
    public string HealthGrade { get; set; }
}

public class HealthMonitoring
{
    public int HeartRate { get; set; }
    public string BloodPressure { get; set; }
    public float OxygenLevel { get; set; }
    public float Temperature { get; set; }
    public string Status { get; set; }
}

public class Family
{
    public string Name { get; set; }
    public string Relationship { get; set; }
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    public string Address { get; set; }
    public bool IsPrimaryContact { get; set; }
}
