# 🎙️ SpeakerID

A speaker identification application written in C# that records audio and compares voices using **MFCC** (Mel-Frequency Cepstral Coefficients) features.

---

## 📌 Table of Contents

- [Overview](#-overview)
- [Tech Stack](#-tech-stack)
- [Repository Structure](#-repository-structure)
- [Getting Started](#-getting-started)

---

## 📖 Overview

SpeakerID provides a desktop GUI for recording audio and identifying speakers. It includes MFCC feature extraction, the main identification algorithms, and a test-case runner for checking results.

---

## 🛠 Tech Stack

- **Language**: C#
- **Framework**: .NET Framework (`Recorder.csproj`)
- **IDE**: Visual Studio / JetBrains Rider (`Recorder.sln`)

---

## 📂 Repository Structure

```text
├── GUI/               # User interface
├── MFCC/              # MFCC feature extraction
├── MainFuctions/      # Core speaker identification functions
├── Recorder/          # Audio recording
├── Properties/        # Project properties
├── Algorithms.cs      # Identification algorithms
├── Program.cs         # Application entry point
├── TestCasesRunner.cs # Runs test cases
├── TestController.cs  # Test coordination
├── TestcaseLoader.cs  # Loads test cases
├── bonus.cs           # Bonus task
├── Recorder.csproj    # Project file
└── Recorder.sln       # Solution file
```

---

## 🚀 Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/IbrahimYasserM/SpeakerID.git
   ```

2. Open `Recorder.sln` in Visual Studio.
3. Build and run the project (**F5**).
