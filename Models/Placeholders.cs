using System.ComponentModel.DataAnnotations;

public class HealthAssessmentReport 
{ 
    [Key]
    public int Id { get; set; }
}

public class MedicalOrder 
{ 
    [Key]
    public int Id { get; set; }
}

public class NursingPlan 
{ 
    [Key]
    public int Id { get; set; }
}

public class ActivityParticipation 
{ 
    [Key]
    public int Id { get; set; }
}

public class DietRecommendation 
{ 
    [Key]
    public int Id { get; set; }
}

public class EmergencySOS 
{ 
    [Key]
    public int Id { get; set; }
} 