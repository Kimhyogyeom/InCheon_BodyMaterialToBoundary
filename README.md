# InCheon_BodyMaterialToBoundary

인천 신체-기계 변환 인터랙티브 애플리케이션

## 프로젝트 개요

사용자가 자신의 신체가 인간인지 로봇인지 선택하는 과정을 시각적으로 보여주는 설치미술/교육용 Unity 애플리케이션입니다.

### 핵심 기능
- 사람에서 로봇으로의 부드러운 시각적 변환 (Dissolve 셰이더)
- 슬라이더를 통한 변환 정도 조절 (0~100%)
- 사람 또는 로봇 선택 및 확정 시스템
- 최종 선택 결과 표시 및 리셋 기능

---

## 조작 방법

| 키 | 기능 |
|----|------|
| `1` | 사람 선택 |
| `2` | 로봇 선택 |
| `3` | 선택 확정 |
| `4` | 최종 확정 |
| `5` | 전체 리셋 |

---

## 프로젝트 구조

```
Assets/
├── Scripts/                          # 핵심 스크립트
│   ├── HumanToRobotTransform.cs     # 변환 로직 (셰이더 제어)
│   ├── SelectButtonHandler.cs       # 선택 UI 관리
│   ├── GaugeWithText.cs             # 슬라이더/텍스트 표시
│   ├── AllGameInit.cs               # 게임 매니저 (리셋 관리)
│   ├── ExitButtonHandler.cs         # 종료 버튼 처리
│   └── CheckButtonHandler.cs        # 예약된 핸들러 (미사용)
│
├── Shaders/                         # 시각 효과
│   ├── HorizontalDissolve.shader    # 인간 사라짐 효과 (좌→우)
│   └── HorizontalDissolveRight.shader # 로봇 나타남 효과 (우→좌)
│
├── Materials/                       # 셰이더 적용 머티리얼
│   ├── HumanMaterial.mat
│   └── AndroidMaterial.mat
│
├── Images/                          # 스프라이트 이미지
│   ├── Human.png
│   ├── Robot.png
│   ├── Background.png
│   └── TextBox.png
│
└── Scenes/
    └── SampleScene.unity            # 메인 씬
```

---

## 주요 스크립트 설명

### HumanToRobotTransform.cs
인간↔로봇 변환 시각 효과를 관리합니다.
- `transformValue`: 0~100 범위의 변환 정도 (0=완전 사람, 100=완전 로봇)
- 두 개의 SpriteRenderer 관리 (인간 이미지, 로봇 이미지)
- 셰이더의 `_DissolveAmount` 파라미터 제어

### SelectButtonHandler.cs
UI 선택 버튼 입력 처리 및 최종 선택 결과 표시
- 선택 시 "당신은 사람 XX% 기계 XX%를 로봇이라고 선택했습니다." 형태로 결과 표시

### GaugeWithText.cs
변환 정도를 슬라이더와 퍼센트 텍스트로 표시

### AllGameInit.cs
게임 전체 리셋 관리 (5번 키로 모든 컴포넌트 초기화)

---

## 데이터 흐름

```
키보드 입력 (1-5)
       ↓
SelectButtonHandler / AllGameInit (입력 처리)
       ↓
HumanToRobotTransform.transformValue 업데이트
       ↓
┌─────────────────────────────────┐
│ GaugeWithText      → Slider + % │
│ HumanToRobotTransform → 셰이더  │
└─────────────────────────────────┘
       ↓
UI 업데이트 (사람↔로봇 시각 변환)
       ↓
SelectButtonHandler (최종 결과 표시)
```

---

## 기술 스택

- **Unity** (InputSystem 사용)
- **TextMeshPro** (고급 텍스트 렌더링)
- **Custom Shader** (Horizontal Dissolve 효과)

---

## 현장 테스트 방안

현재 센서 연동 없이 키보드로만 동작합니다. 현장 테스트를 위한 옵션:

### 1. 키보드 테스트
현재 구현된 키보드 입력(1-5)으로 모든 기능 테스트 가능

### 2. 센서 시뮬레이터 추가 (예정)
- 마우스 드래그로 변환값 조절
- 화면 터치로 선택
- 타이머 기반 자동 시퀀스

### 3. 외부 입력 장치 연동 (예정)
- OSC/MIDI 입력 지원
- 네트워크 리모트 컨트롤

---

## 향후 개발 계획

- [ ] 센서 시뮬레이터 구현
- [ ] 외부 센서 연동 (모션 센서, 터치 패널 등)
- [ ] 네트워크 리모트 컨트롤
- [ ] CheckButtonHandler 기능 구현

---

## 라이선스

(라이선스 정보를 여기에 추가하세요)
