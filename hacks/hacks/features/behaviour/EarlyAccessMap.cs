using System;
using blazey.features;

namespace hacks.features.behaviour
{
    public class EarlyAccessMap : IFeatureMap
    {
        private readonly IHttpContextAccessor _accessor;

        public EarlyAccessMap(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public Type ImplementationType()
        {
            return _accessor.IpAddress == "192.168.0.7"
                ? typeof (Experimental)
                : typeof (DefaultBehaviour);
        }

        public Type FeatureType => typeof (IFeature);
    }

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
}