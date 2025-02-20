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
void Restore(void* windowHandle);

MsgResult ShowDialog(const char *title, const char *message, MsgBtns btn, MessageType messageType, const char *iconName);
void OpenFileDialog(void* windowHandle, char* outputPath, size_t maxPathLength);
void OpenMultipleFileDialog(void* windowHandle, char** outputPaths, size_t maxPaths, size_t maxPathLength);
void OpenFolderDialog(void* windowHandle, char* outputPath, size_t maxPathLength);
void GetScreenSize(int* width, int* height);
void Invoke(void (^block)(void));

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

void Restore(void* windowHandle) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        [window makeKeyAndOrderFront:nil]; // 恢复并显示窗口
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

// 单选文件选择器
void OpenFileDialog(void* windowHandle, char* outputPath, size_t maxPathLength) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;

    NSOpenPanel *openPanel = [NSOpenPanel openPanel];
    [openPanel setAllowsMultipleSelection:NO]; // 不允许多选
    [openPanel setCanChooseDirectories:NO]; // 不允许选择目录

    // 显示文件选择器并处理用户选择的文件
    [openPanel beginSheetModalForWindow:window completionHandler:^(NSModalResponse result) {
        if (result == NSModalResponseOK) {
            NSArray *urls = [openPanel URLs]; // 获取选择的文件URL
            if (urls.count > 0) {
                NSURL *selectedFileURL = urls[0];
                NSString *filePath = selectedFileURL.path;

                // 将文件路径复制到输出参数
                strncpy(outputPath, [filePath UTF8String], maxPathLength - 1);
                outputPath[maxPathLength - 1] = '\0'; // 确保字符串以 null 结尾
            }
        }
    }];
}

// 多选文件选择器
void OpenMultipleFileDialog(void* windowHandle, char** outputPaths, size_t maxPaths, size_t maxPathLength) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;

    NSOpenPanel *openPanel = [NSOpenPanel openPanel];
    [openPanel setAllowsMultipleSelection:YES]; // 允许多选
    [openPanel setCanChooseDirectories:NO]; // 不允许选择目录

    // 显示文件选择器并处理用户选择的文件
    [openPanel beginSheetModalForWindow:window completionHandler:^(NSModalResponse result) {
        if (result == NSModalResponseOK) {
            NSArray *urls = [openPanel URLs]; // 获取选择的文件URL
            size_t count = MIN(urls.count, maxPaths); // 确保不超过最大路径数

            for (size_t i = 0; i < count; i++) {
                NSURL *selectedFileURL = urls[i];
                NSString *filePath = selectedFileURL.path;

                // 将文件路径复制到输出参数
                strncpy(outputPaths[i], [filePath UTF8String], maxPathLength - 1);
                outputPaths[i][maxPathLength - 1] = '\0'; // 确保字符串以 null 结尾
            }
        }
    }];
}

void OpenFolderDialog(void* windowHandle, char* outputPath, size_t maxPathLength) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;

    NSOpenPanel *openPanel = [NSOpenPanel openPanel];
    [openPanel setAllowsMultipleSelection:NO]; // 不允许多选
    [openPanel setCanChooseFiles:NO]; // 不允许选择文件
    [openPanel setCanChooseDirectories:YES]; // 允许选择目录

    // 显示文件选择器并处理用户选择的文件夹
    [openPanel beginSheetModalForWindow:window completionHandler:^(NSModalResponse result) {
        if (result == NSModalResponseOK) {
            NSArray *urls = [openPanel URLs]; // 获取选择的文件夹URL
            if (urls.count > 0) {
                NSURL *selectedFolderURL = urls[0];
                NSString *folderPath = selectedFolderURL.path;

                // 将文件夹路径复制到输出参数
                strncpy(outputPath, [folderPath UTF8String], maxPathLength - 1);
                outputPath[maxPathLength - 1] = '\0'; // 确保字符串以 null 结尾
            }
        }
    }];
}

void GetScreenSize(int* width, int* height) {
    NSRect screenFrame = [[NSScreen mainScreen] frame];
    if (width != NULL) {
        *width = screenFrame.size.width;
    }
    if (height != NULL) {
        *height = screenFrame.size.height;
    }
}

void Invoke(void (^block)(void)) {
    if ([NSThread isMainThread]) {
        // 如果当前已经在主线程上，直接执行
        block();
    } else {
        // 否则，调度到主线程
        dispatch_async(dispatch_get_main_queue(), block);
    }
}
