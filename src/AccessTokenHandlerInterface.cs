using System;

namespace OblioSoftware;

interface AccessTokenHandlerInterface
{
    public AccessToken? Get();

    public void Set(AccessToken accessToken);
}