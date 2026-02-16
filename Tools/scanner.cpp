#include <windows.h>
#include <shobjidl.h>
#include <gdiplus.h>
#include <filesystem>
#include <fstream>
#include <vector>
#include <string>
#include <iostream>

namespace fs = std::filesystem;

std::string WStringToUtf8(const std::wstring& w) {
    if (w.empty()) return {};
    int size = WideCharToMultiByte(CP_UTF8, 0, w.data(), (int)w.size(),
        nullptr, 0, nullptr, nullptr);
    std::string out(size, 0);
    WideCharToMultiByte(CP_UTF8, 0, w.data(), (int)w.size(),
        out.data(), size, nullptr, nullptr);
    return out;
}

std::string EscapeJson(const std::string& s) {
    std::string o;
    for (char c : s) {
        switch (c) {
            case '\\': o += "\\\\"; break;
            case '"':  o += "\\\""; break;
            case '\n': o += "\\n"; break;
            case '\r': o += "\\r"; break;
            case '\t': o += "\\t"; break;
            default:   o += c;
        }
    }
    return o;
}

std::string EscapeJson(const std::wstring& w) {
    return EscapeJson(WStringToUtf8(w));
}

struct ShortcutInfo {
    std::wstring name;
    std::wstring lnkPath;
    std::wstring target;
    std::wstring args;
    std::wstring iconPath;
    int iconIndex = 0;
};

bool ResolveLnk(const fs::path& lnk, ShortcutInfo& out) {
    IShellLinkW* link = nullptr;
    IPersistFile* file = nullptr;

    if (FAILED(CoCreateInstance(CLSID_ShellLink, nullptr,
        CLSCTX_INPROC_SERVER, IID_IShellLinkW, (void**)&link)))
        return false;

    if (FAILED(link->QueryInterface(IID_IPersistFile, (void**)&file))) {
        link->Release();
        return false;
    }

    if (FAILED(file->Load(lnk.c_str(), STGM_READ))) {
        file->Release();
        link->Release();
        return false;
    }

    wchar_t buf[MAX_PATH];

    link->GetPath(buf, MAX_PATH, nullptr, SLGP_RAWPATH);
    out.target = buf;

    link->GetArguments(buf, MAX_PATH);
    out.args = buf;

    link->GetIconLocation(buf, MAX_PATH, &out.iconIndex);
    out.iconPath = buf;

    out.lnkPath = lnk.wstring();
    out.name = lnk.stem().wstring();

    file->Release();
    link->Release();
    return !out.lnkPath.empty();
}

int wmain() {
    CoInitialize(nullptr);

    Gdiplus::GdiplusStartupInput gdiInput;
    ULONG_PTR gdiToken;
    Gdiplus::GdiplusStartup(&gdiToken, &gdiInput, nullptr);

    fs::create_directories("Tools/icons");

    std::vector<ShortcutInfo> apps;

    std::vector<fs::path> roots = {
        fs::path(_wgetenv(L"APPDATA")) / L"Microsoft\\Windows\\Start Menu\\Programs",
        L"C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs"
    };

    for (auto& root : roots) {
        if (!fs::exists(root)) continue;
        for (auto& e : fs::recursive_directory_iterator(root)) {
            if (e.path().extension() == L".lnk") {
                ShortcutInfo info;
                if (ResolveLnk(e.path(), info))
                    apps.push_back(info);
            }
        }
    }

    std::ofstream json("Tools/apps.json", std::ios::binary);
    json << "{\n  \"apps\": [\n";

    for (size_t i = 0; i < apps.size(); ++i) {
        auto& a = apps[i];

        json << "    {\n";
        json << "      \"name\": \"" << EscapeJson(a.name) << "\",\n";
        json << "      \"lnk\": \"" << EscapeJson(a.lnkPath) << "\",\n";
        json << "      \"target\": \"" << EscapeJson(a.target) << "\",\n";
        json << "      \"args\": \"" << EscapeJson(a.args) << "\",\n";
        json << "    }" << (i + 1 < apps.size() ? "," : "") << "\n";
    }

    json << "  ]\n}\n";
    json.close();

    Gdiplus::GdiplusShutdown(gdiToken);
    CoUninitialize();
    return 0;
}

