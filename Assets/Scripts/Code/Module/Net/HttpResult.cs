namespace TaoTie
{
    public class HttpResult
    {
        public string msg;
        public int code;
        public bool status;
    }
    public class HttpResult<T>
    {
        public string msg;
        public int code;
        public bool status;
        public T data;
    }
}