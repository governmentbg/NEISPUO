namespace MON.Models.Diploma
{
    public enum DiplomaFinaliziationStepEnum
    {
        ResetStepsNumber = 0,
        IsFinalizedDiplomaStepNumber = 1,
        // Подписване на диплома, която автоматично включва всички останали в себе си
        IsSignedDiplomaStepNumber = 2,
        IsPublicDiplomaStepNumber = 3
    }
}