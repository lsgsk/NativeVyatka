using System;
using Android.Content;
using Uri = Android.Net.Uri;

namespace NativeVyatkaAndroid
{   
    [ContentProvider(new[] { SyncConstants.CONTENT_AUTHORITY })]
    public class RecordsContentProvider : ContentProvider
    {
        public RecordsContentProvider()
        {
        }

        public override int Delete(Uri uri, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }

        public override string GetType(Uri uri)
        {
            throw new NotImplementedException();
        }

        public override Uri Insert(Uri uri, ContentValues values)
        {
            throw new NotImplementedException();
        }

        public override bool OnCreate()
        {
            return true;
        }

        public override Android.Database.ICursor Query(Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            throw new NotImplementedException();
        }

        public override int Update(Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }
    }
}

