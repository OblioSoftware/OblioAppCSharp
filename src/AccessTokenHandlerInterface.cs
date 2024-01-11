using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OblioSoftware;

interface AccessTokenHandlerInterface
{
    public AccessToken? Get();

    public void Set(AccessToken accessToken);
}