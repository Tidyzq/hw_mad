#include "HelloWorldScene.h"
#include "cocostudio/CocoStudio.h"
#include "ui/CocosGUI.h"
#include <string>
#include <random>
#include <algorithm>

USING_NS_CC;

using namespace cocostudio::timeline;

Scene* HelloWorld::createScene()
{
    // 'scene' is an autorelease object
    auto scene = Scene::create();
    
    // 'layer' is an autorelease object
    auto layer = HelloWorld::create();

    // add layer as a child to scene
    scene->addChild(layer);

    // return the scene
    return scene;
}

// on "init" you need to initialize your instance
bool HelloWorld::init()
{
    // 1. super init first
    if ( !Layer::init() )
    {
        return false;
    }
    
    Size visibleSize = Director::getInstance()->getVisibleSize();
    Vec2 origin = Director::getInstance()->getVisibleOrigin();

    /////////////////////////////
    // 2. add a menu item with "X" image, which is clicked to quit the program
    //    you may modify it.

    // add a "close" icon to exit the progress. it's an autorelease object
    auto closeItem = MenuItemImage::create(
                                           "CloseNormal.png",
                                           "CloseSelected.png",
                                           CC_CALLBACK_1(HelloWorld::menuCloseCallback, this));
    
    closeItem->setScale(50/closeItem->getContentSize().width, 50/closeItem->getContentSize().height);
	closeItem->setPosition(Vec2(origin.x + visibleSize.width - closeItem->getScaleX()*closeItem->getContentSize().width/2, origin.y + closeItem->getScaleY()*closeItem->getContentSize().height/2));
    
    auto githubItem = MenuItemImage::create("GitHub-Light.png", "GitHub-Light.png", CC_CALLBACK_1(HelloWorld::githubCallback, this));
    
    githubItem->setScale(50/githubItem->getContentSize().width, 50/githubItem->getContentSize().height);
    githubItem->setPosition(Vec2(origin.x + visibleSize.width - githubItem->getScaleX()*githubItem->getContentSize().width/2, origin.y + githubItem->getScaleY()*githubItem->getContentSize().height*3/2));

    // create menu, it's an autorelease object
    auto menu = Menu::create(closeItem, githubItem, NULL);
    menu->setPosition(Vec2::ZERO);
    this->addChild(menu, 1);

    /////////////////////////////
    // 3. add your codes below...

    // add a label shows "Hello World"
    // create and initialize a label

    // add "HelloWorld" splash screen"
    auto sprite = Sprite::create("my.png");

    // position the sprite on the center of the screen
    sprite->setScale(200/sprite->getContentSize().width, 200/sprite->getContentSize().height);
    sprite->setPosition3D(Vec3(visibleSize.width/2 + origin.x, visibleSize.height/2 + origin.y, 0));

    // add the sprite as a child to this layer
    this->addChild(sprite, 0);
    
    std::vector<std::string> texts {
        "Tidyzq",
        "郑齐",
        "14331385"
    };
    
    const float pi = 3.14159265358979323846264338327950;
    const float closeness = 10.0;
    
    int k = 0;
    float lra = 0.0;
    for (int i = 0; i <= closeness; ++i) {
        int mj = ceil(closeness * sinf(i * pi / closeness) * sinf(i * pi / closeness));
        mj = mj ? mj : 1;
        for (int j = 0; j < mj; ++j) {
            auto label = Label::createWithSystemFont(texts[k], "Consolas", 24);
            label->setAlignment(cocos2d::TextHAlignment::CENTER);
            auto sphere = SphereItem(origin.x + visibleSize.width/2, origin.y + visibleSize.height/2, 0, 250, j / static_cast<float>(mj) + lra/2, i / closeness);
            auto vec = sphere.getVec();
            label->setPosition3D(vec);
            label->setGlobalZOrder(vec.z);
            addChild(label, vec.z);
            Labels.push_back(std::make_pair(label, sphere));
            k = (k + 1) % texts.size();
        }
        lra = 1.0 / mj;
    }
    
    this->schedule([this](float dt) {
        for (auto it = this->Labels.begin(); it != this->Labels.end(); ++it) {
            it->second.ry = it->second.ry + 0.0015;
            if (it->second.ry >= 1) it->second.ry -= 1;
            it->second.rx = it->second.rx + 0.0005;
            if (it->second.rx >= 1) it->second.rx -= 1;
            it->second.rz = it->second.rz + 0.0003;
            if (it->second.rz >= 1) it->second.rz -= 1;
            auto vec = it->second.getVec();
            it->first->setPosition3D(vec);
            it->first->setGlobalZOrder(vec.z);
        }
    }, 0.02, "HelloWorld::scheduleSpin");

    return true;
}

void HelloWorld::menuCloseCallback(cocos2d::Ref* pSender) {
    Director::getInstance()->end();
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    exit(0);
#endif
}

void HelloWorld::githubCallback(cocos2d::Ref* pSender) {
    Application::sharedApplication()->openURL("http://github.com/tidyzq");
}