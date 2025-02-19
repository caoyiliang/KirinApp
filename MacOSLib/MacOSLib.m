#import <Cocoa/Cocoa.h>

typedef NS_ENUM(NSInteger, MsgResult){
    OK,
    Cancel,
    Yes,
    No
}

typedef NS_ENUM(NSInteger, MsgBtns) {
    OK,
    OKCancel,
    YesNo,
    YesNoCancel
};

typedef NS_ENUM(NSInteger, MessageType) {
    Info,
    Warning,
    Question,
    Error,
    Other
};

#ifdef __cplusplus
extern "C" {
#endif

// 声明 C 函数
void* CreateWindow(int left, int top, int width, int height, int chromeless);
void SetTitle(void* windowHandle, const char *title);
void SetIcon(void* windowHandle, const char *path);
void ShowWindow(void* windowHandle, int show);
void FocusWindow(void* windowHandle);
void RunLoop();
void Move(void* windowHandle, int left, int top);
void ChangeSize(void* windowHandle, int width, int height);
void TopMost(void* windowHandle, int topMost);
void Minimize(void* windowHandle);
void Maximize(void* windowHandle);

MsgResult ShowDialog(const char *title, const char *message, MsgBtns btn, MessageType messageType, const char *iconName)
#ifdef __cplusplus
}
#endif

// 窗口相关的变量
NSWindow *g_window = nil;

@interface MyWindowDelegate : NSObject <NSWindowDelegate>
@end

@implementation MyWindowDelegate
- (void)windowWillClose:(NSNotification *)notification {
    [NSApp terminate:nil];
}
@end

void* CreateWindow(int left, int top, int width, int height, int chromeless) {
    @autoreleasepool {
        NSRect frame = NSMakeRect(left, top, width, height);
        MyWindowDelegate *delegate = [[MyWindowDelegate alloc] init];

        NSUInteger styleMask = NSWindowStyleMaskTitled | NSWindowStyleMaskClosable | NSWindowStyleMaskResizable;
        if (chromeless) {
            styleMask = NSWindowStyleMaskBorderless; // 无边框窗口
        }

        g_window = [[NSWindow alloc] initWithContentRect:frame
                                                styleMask:styleMask
                                                  backing:NSBackingStoreBuffered
                                                    defer:NO];
        [g_window setDelegate:delegate];
        return (__bridge void*)g_window; // 返回窗口句柄
    }
}

void SetTitle(void* windowHandle, const char *title) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        NSString *nsTitle = [NSString stringWithUTF8String:title];
        [window setTitle:nsTitle]; // 设置窗口标题
    }
}

void SetIcon(void* windowHandle, const char *path) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        NSImage *icon = [[NSImage alloc] initWithContentsOfFile:[NSString stringWithUTF8String:path]];
        if (icon) {
            [window setMiniwindowImage:icon]; // 设置最小化窗口图标
        } else {
            NSLog(@"无法加载图像，路径: %s", path);
        }
    }
}

void ShowWindow(void* windowHandle, int show) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        if (show) {
            [window makeKeyAndOrderFront:nil]; // 显示窗口
        } else {
            [window orderOut:nil]; // 隐藏窗口
        }
    }
}

void FocusWindow(void* windowHandle) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        [window makeKeyAndOrderFront:nil]; // 使窗口获得焦点
    }
}

void Move(void* windowHandle, int left, int top) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        NSRect frame = window.frame;
        frame.origin.x = left;
        frame.origin.y = top;
        [window setFrame:frame display:YES animate:YES]; // 移动窗口
    }
}

void ChangeSize(void* windowHandle, int width, int height) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        NSRect frame = window.frame;
        frame.size.width = width;
        frame.size.height = height;
        [window setFrame:frame display:YES animate:YES]; // 更改窗口大小
    }
}

void TopMost(void* windowHandle, int topMost) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        [window setLevel:topMost ? NSStatusWindowLevel : NSNormalWindowLevel]; // 设置窗口置顶
    }
}

void Minimize(void* windowHandle) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        [window miniaturize:nil]; // 最小化窗口
    }
}

void Maximize(void* windowHandle) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        [window makeKeyAndOrderFront:nil]; // 确保窗口在前
        [window zoom:nil]; // 最大化窗口
    }
}

void RunLoop() {
    [[NSApplication sharedApplication] run]; // 启动事件循环
}

NSImage* GetIcon(NSString *path) {
    NSImage *image = [[NSImage alloc] initWithContentsOfFile:path];
    if (!image) {
        NSLog(@"无法加载图像，路径: %@", path);
    }
    return image;
}

MsgResult ShowDialog(const char *title, const char *message, MsgBtns btn, MessageType messageType, const char *iconName) {
    @autoreleasepool {
        NSAlert *alert = [[NSAlert alloc] init];
        [alert setMessageText:[NSString stringWithUTF8String:title]];
        [alert setInformativeText:[NSString stringWithUTF8String:message]];

        // 根据按钮类型添加按钮
        if (btn == OK) {
            [alert addButtonWithTitle:@"确定"];
        } else if (btn == OKCancel) {
            [alert addButtonWithTitle:@"确定"];
            [alert addButtonWithTitle:@"取消"];
        }else if (btn == YesNo) {
            [alert addButtonWithTitle:@"是"];
            [alert addButtonWithTitle:@"否"];
        }else if (btn == YesNoCancel) {
            [alert addButtonWithTitle:@"是"];
            [alert addButtonWithTitle:@"否"];
            [alert addButtonWithTitle:@"取消"];
        }

        // 根据消息类型设置图标
        if (messageType == Info) {
            [alert setIcon:[NSImage imageNamed:NSImageNameInfo]];
        } else if (messageType == Warning) {
            [alert setIcon:[NSImage imageNamed:NSImageNameWarning]];
        } else if (messageType == Error) {
            [alert setIcon:[NSImage imageNamed:NSImageNameError]];
        }else if(messageType == Question){
            [alert setIcon:[NSImage imageNamed:NSImageNameInfo]];
        }else{
            [alert setIcon:[NSImage imageNamed:NSImageNameInfo]];
        }

        if (iconName) {
            NSString *iconNameStr = [NSString stringWithUTF8String:iconName];
            [alert setIcon:[NSImage imageNamed:iconNameStr]];
        }

        // 显示对话框并获取用户响应
        NSApplication.ModalResponse response = [alert runModal];

        // 根据响应返回相应的枚举值
        switch (btn) {
            case OK:
                return OK;
            case OKCancel:
                return (response == NSAlertFirstButtonReturn) ? OK : Cancel;
            case YesNo:
                return (response == NSAlertFirstButtonReturn) ? Yes : No;
            case YesNoCancel:
                if (response == NSAlertFirstButtonReturn) return Yes; // “是”
                if (response == NSAlertSecondButtonReturn) return No; // “否”
                return Cancel; // “取消”
        }
        return OK;
    }
}