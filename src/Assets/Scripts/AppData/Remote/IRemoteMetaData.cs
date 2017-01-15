﻿using PatchKit.Api.Models;

namespace PatchKit.Unity.Patcher.AppData.Remote
{
    public interface IRemoteMetaData
    {
        int GetLatestVersionId();

        Api.Models.App GetAppInfo();

        AppContentSummary GetContentSummary(int versionId);

        AppDiffSummary GetDiffSummary(int versionId);

        string GetKeySecret(string key);
    }
}