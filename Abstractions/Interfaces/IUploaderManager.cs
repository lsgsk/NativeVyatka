namespace Abstractions
{
    public interface IUploaderManager
    {
        void ReqestBulialsUploading();
        void NotifiController(bool uploadResult);        
    }
}

