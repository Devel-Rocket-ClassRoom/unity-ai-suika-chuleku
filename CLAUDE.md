# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 저장소 구조

저장소 루트는 Unity 프로젝트 루트가 **아니다**. Unity 프로젝트는 한 단계 아래에 있다:

```
unity-ai-suika-chuleku/        ← 저장소 루트, Claude Code는 여기서 실행됨
├── .claude/                   ← Claude Code 설정 (Unity MCP 도구 사전 허용됨)
├── Docs/
│   └── GDD.md                 ← 게임 설계 문서 — 게임 로직 작업 전 반드시 읽을 것
└── SubakGame/                 ← Unity 프로젝트 루트 (Unity Editor에서 이걸 연다)
    ├── Assets/
    ├── Packages/manifest.json
    ├── ProjectSettings/
    └── SubakGame.slnx         ← .NET 솔루션 (신규 SLNX 포맷)
```

Unity 에셋/스크립트 경로를 언급할 때는 `SubakGame/`을 기준으로 한다 (예: `SubakGame/Assets/Scenes/SubakGame.unity`).

## 프로젝트 정체성

- **목표**: *수박게임* (Suika Game) 클론 — 2D 물리 머지 퍼즐. 같은 과일을 떨어뜨려 합치고 최종적으로 수박을 만드는 게임.
- **엔진**: Unity **6000.3.15f1** (Unity 6) + **URP 2D Renderer**. 버전 확인은 `SubakGame/ProjectSettings/ProjectVersion.txt`.
- **입력**: 신 Input System (`com.unity.inputsystem` 1.19.0). 레거시 `Input.GetKey` 사용 금지 — `InputSystem_Actions.inputactions` 활용.
- **렌더링**: 2D URP (`Settings/Renderer2D.asset`, `UniversalRP.asset`). 스프라이트 기반, 3D 메시 아님. 현재 씬에 있는 큐브들은 임시 프리미티브임.
- **씬**: `Assets/Scenes/SubakGame.unity`가 게임플레이 씬, `SampleScene.unity`는 기본 생성된 미사용 씬.

전체 게임 설계(과일 체인, 점수, 모드, 마일스톤)는 `Docs/GDD.md`에 있다. 게임플레이 규칙을 변경하기 전에 반드시 읽을 것 — 단일 진실 공급원(source of truth).

## 작성 언어

이 저장소의 모든 문서(`*.md`, 설계 문서, 주석성 메모)는 **한국어**로 작성한다. 코드 식별자, 패키지명, API 키워드, 파일 경로는 영어를 유지한다.

## Unity MCP 워크플로우

`com.unity.ai.assistant` 2.7.0-pre.3 패키지가 설치되어 있어 MCP로 Unity Editor를 직접 제어할 수 있다. **사용자에게 Editor에서 직접 해달라고 요청하기 전에 MCP 도구를 먼저 사용하라.**

도구 빠른 참조:
- `mcp__unity-mcp__Unity_RunCommand` — Editor 안에서 C# 컴파일·실행. 클래스명은 반드시 `internal class CommandScript : IRunCommand`. Undo 지원을 위해 `result.RegisterObjectCreation/Modification`, `result.DestroyObject` 사용. 오브젝트 참조 로깅은 `result.Log("{0}", obj)`.
- `mcp__unity-mcp__Unity_ManageGameObject` — C# 작성 없이 GameObject/Component를 생성·조회·수정·삭제. 단순 작업에 더 빠름.
- `mcp__unity-mcp__Unity_ManageScene` — `GetHierarchy`, `Load`, `Save`, `Create`, `GetActive`.
- `mcp__unity-mcp__Unity_GetConsoleLogs` — 컴파일 오류/경고 확인.
- `mcp__unity-mcp__Unity_SceneView_Capture2DScene` / `CaptureMultiAngleSceneView` / `Unity_Camera_Capture` — 시각 검증. **UI/씬 변경 후 결과를 막연히 보고하지 말고 캡처로 확인할 것**.
- `mcp__unity-mcp__Unity_ReadResource` — `unity://` URI로 `Assets/` 하위 에셋 읽기.

### MCP가 `Connection revoked` 오류로 실패할 때

Unity Editor의 앱별 승인은 Claude Code 실행파일 해시에 민감하다. 도구 호출에 `Connection revoked. Go to Unity Editor > Project Settings > AI > Unity MCP to change approval.`이 뜨면:
1. 사용자에게 Editor의 **Project Settings → AI → Unity MCP Server**를 열게 한다.
2. **Revoke** 클릭 → Claude Code에서 `/mcp`로 재연결 → 새로 뜨는 요청에 **Accept** 클릭.
3. Unity가 실행 중이어야 모든 MCP 도구가 동작한다.

## 일반 명령어

이 저장소에는 CLI 기반 빌드/테스트/린트 파이프라인이 없다. 모든 작업은 Unity Editor 또는 MCP를 통해 수행한다.

| 작업 | 방법 |
|------|------|
| 프로젝트 열기 | Unity Hub → Open → `SubakGame/` 선택 |
| 게임 실행 | Editor Play 모드 (헤드리스 러너 미설정) |
| 콘솔 오류 확인 | `Unity_GetConsoleLogs` MCP 도구 |
| 스크립트화된 Editor 로직 실행 | `Unity_RunCommand` + `IRunCommand` 템플릿 |
| 씬 내용 조회/수정 | `Unity_ManageGameObject` / `Unity_ManageScene` |
| 변경 결과 시각 확인 | `Unity_SceneView_*Capture*` 또는 `Unity_Camera_Capture` |

## 향후 작업 시 유의점

- **`.gitignore`가 아직 없다.** 그대로 커밋하면 `SubakGame/Library/`, `Temp/`, `Logs/`, `UserSettings/` 같은 머신 종속·대용량 폴더까지 끌려 들어간다. 의미 있는 첫 커밋 전에 Unity용 `.gitignore`를 추가하라.
- **게임 스크립트가 아직 없다** (`Assets/**/*.cs`가 비어 있음). 프로젝트는 GDD 단계이며 M1 프로토타입 작업이 시작되지 않았다. 스크래폴딩 시 `Docs/GDD.md` §12.2에서 제안하는 폴더 구조(`Assets/_Project/Scripts/...`)를 따른다.
- **2D 물리 사용**: `Rigidbody2D`, `CircleCollider2D`. GDD가 성능·안정성 이유로 2D를 명시했고, 프로젝트는 이미 URP 2D 구성이다. 3D 물리 컴포넌트를 섞지 말 것.
