using System;

namespace Server.Common;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}
