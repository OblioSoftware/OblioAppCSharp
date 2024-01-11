using System;

namespace OblioSoftware;

interface IAccessTokenHandler
{
    public AccessToken? Get();

    public void Set(AccessToken accessToken);
}