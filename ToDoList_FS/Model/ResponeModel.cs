namespace ToDoList_FS.Model
{
    public class ResponeModel
    {
        public bool Status { get; set; }
    }

    public class SuccessResponeModel<T> : ResponeModel
    {
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
    public class ErrorResponeModel : ResponeModel
    {
        public string? ErrorMessage { get; set; }
    }
}