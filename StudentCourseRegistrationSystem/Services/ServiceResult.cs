namespace StudentCourseRegistrationSystem.Services
{
    /// <summary>
    /// Tüm servis metodlarının döndürdüğü sonuç nesnesi.
    /// Exception fırlatmak yerine Success/Failure ile sonuç döndürülür.
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private ServiceResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static ServiceResult SuccessResult(string message = "İşlem başarıyla tamamlandı.")
            => new ServiceResult(true, message);

        public static ServiceResult Failure(string message)
            => new ServiceResult(false, message);
    }
}
