using System;

namespace NativeVyatkaCore
{
    public interface IUploaderManager
    {
        void ReqestBulialsUploading();
        void NotifiController(bool uploadResult);        
    }
}

