//
//  SphereItem.hpp
//  helloworld
//
//  Created by Tidyzq on 16/4/15.
//
//

#ifndef SphereItem_h
#define SphereItem_h

#include <cstdio>
#include <cmath>
#include "cocos2d.h"

struct SphereItem {
    float originX, originY, originZ;
    float x, y, z, rx, ry, rz;
    
    SphereItem(float ox, float oy, float oz, float rr, float raa, float ell);
    
    cocos2d::Vec3 getVec() const;
};

#endif /* SphereItem_h */
