# 咩计时

面向导播、演讲和 PPT 放映场景的轻量级双窗口计时工具。

**Windows 10+ · 单文件 · 免安装**

**[前往 Releases 下载最新版本](https://github.com/zanjie1999/meTimer/releases/latest)**

<img width="700" alt="image" src="https://github.com/user-attachments/assets/2c4a17b5-3631-463e-b412-0ad9a95051c1" />

## 功能

- **双窗口**：窗口 1 为控制台；窗口 2 为无边框计时窗，始终置顶，大小可调，可切换显示小时。
- **倒计时 / 顺计时**：倒计时默认 15:00；顺计时也可以设置计划时长，设为 0 表示不限时长。
- **开始 / 暂停 / 继续**：开始和暂停使用同一个按钮；暂停时控制窗计时区变为橙色。
- **超时提醒**：倒计时归零后，计时窗停在 `00:00:00`，控制窗变红并继续累计超时；顺计时超过设定时长后，控制窗变红，计时继续。
- **多显示器**：计时窗默认显示在副屏左上角，可在主屏 / 副屏之间切换，支持拖动和拖动边缘缩放。
- **显示控制**：可随时显示 / 隐藏计时窗；计时窗显示时，按钮会有醒目的状态色。
- **计时记录**：记录开始时间、计划时长、实际用时和超时时长；按序号倒序展示，可逐条删除。
- **复制记录**：一键以 TSV 格式复制到剪贴板（序号 1 在最前），可直接粘贴到 Excel 等表格。

## 使用

1. 在 [Releases](https://github.com/zanjie1999/meTimer/releases/latest) 下载 `meTimer.exe`。
2. 在窗口 1 设置计时模式和时长，点击“开始计时”。
3. 暂停和继续使用同一个按钮；结束本轮点击“停止”。
4. 点击计时窗控制行最右侧的“复制记录到剪贴板”，可将记录粘贴到表格中。
5. 导播可在窗口 1 中实时查看时间，使用“隐藏计时窗 / 显示计时窗”控制副屏画面。

计时记录只保存在内存中，关闭程序后会清空，请及时复制

### 协议 咩License

使用此项目视为您已阅读并同意遵守 [此LICENSE](https://github.com/zanjie1999/LICENSE)   
Using this project is deemed to indicate that you have read and agreed to abide by [this LICENSE](https://github.com/zanjie1999/LICENSE)   
