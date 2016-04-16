//
//  SphereItem.cpp
//  helloworld
//
//  Created by Tidyzq on 16/4/15.
//
//

#include "SphereItem.h"

const float pi = 3.14159265358979323846264338327950;

SphereItem::SphereItem(float ox, float oy, float oz, float rr, float ra, float el) :
    originX(ox), originY(oy), originZ(oz) {
    float sigma = 2 * pi * ra, phi = pi * el;
    x = rr * sinf(sigma) * sinf(phi);
    y = rr * cosf(phi);
    z = rr * cosf(sigma) * sinf(phi);
}

cocos2d::Vec3 SphereItem::getVec() const {
    
    float roateX = 2 * pi * rx, roateY = 2 * pi * ry, roateZ = 2 * pi * rz;
    
    float ny = y * cosf(roateX) - z * sinf(roateX);
    float nz = y * sinf(roateX) + z * cosf(roateX);
    
    float nx = x * cosf(roateY) - nz * sinf(roateY);
    float nnz = x * sinf(roateY) + nz * cosf(roateY);
    
    float nny = ny * cosf(roateZ) - nx * sinf(roateZ);
    float nnx = ny * sinf(roateZ) + nx * cosf(roateZ);
    
    return cocos2d::Vec3(originX + nnx, originY + nny, originZ + nnz);
}