# 🎙️ SpeakerID

A speaker identification system in C#. It records a voice, converts it to **MFCC** features, and identifies the speaker by matching it against enrolled voices using **Dynamic Time Warping (DTW)**.

<!-- Add a screenshot of the recorder window here, e.g. ![Recorder](docs/recorder.png) -->

---

## 📌 Table of Contents

- [How It Works](#-how-it-works)
- [Features](#-features)
- [Usage](#-usage)
- [Tech Stack](#-tech-stack)
- [Repository Structure](#-repository-structure)
- [Getting Started](#-getting-started)

---

## 🔬 How It Works

1. **Record or load audio**: a Windows Forms recorder captures audio, and WAV files can also be loaded.
2. **Remove silence** (optional): silent segments are trimmed before feature extraction.
3. **Extract MFCC features**: each audio frame becomes a vector of **13 coefficients**, so a voice is a sequence of 13-dimensional frames.
4. **Match with DTW**: the input sequence is compared with every enrolled sequence. Frame distance is Euclidean, and DTW aligns sequences of different lengths and speaking speeds.
5. **Pick the closest speaker**: the enrolled speaker with the smallest DTW cost is returned.

### Algorithms

| Algorithm | File | Description |
| --- | --- | --- |
| **DTW** | `Algorithms.cs` | Standard DTW with rolling arrays, so memory is O(m) instead of O(n·m). Allows steps that skip a frame. |
| **DTW with pruning** | `Algorithms.cs` | Restricts the search to a band of width `W` around the diagonal, which is faster at a small accuracy cost. |
| **Synchronized DTW (bonus)** | `bonus.cs` | A best-first, Dijkstra-style search that explores all enrolled templates at once using a priority queue (`SortedSet`) and stops when the first template reaches the end. |

---

## ✨ Features

- Enroll speakers by name. Enrollments are saved to `AudioPaths.txt` and reloaded on startup.
- Identify a speaker from a new recording, with optional silence removal and optional pruning.
- Test mode that reports **accuracy and running time**, with and without pruning, on sample and complete test sets.

---

## 🕹 Usage

The program starts with a console menu:

```text
Run the Program (P) or run the Tests (T)
```

**Program mode (`P`)**

- `A`: add a new voice. Enter a name, then record or load audio in the recorder window.
- `I`: identify a voice. Choose whether to remove silence and whether to prune (and the pruning width), then record or load audio. The program prints the matched speaker and the DTW cost.

**Test mode (`T`)**

- Sample tests: compare DTW with and without pruning, or match one input against the templates.
- Complete tests (1, 2 or 3): run a full test case and print accuracy and time.

> Test mode reads the course test-case folders (for example `SAMPLE\Input sample\...`), which are not included in this repository.

---

## 🛠 Tech Stack

- **Language**: C# (.NET Framework, Windows, x86)
- **UI**: Windows Forms
- **Audio**: [Accord.NET](http://accord-framework.net) (audio capture and WAV decoding)
- **Algorithms**: MFCC, DTW

---

## 📂 Repository Structure

```text
├── Algorithms.cs        # Euclidean distance, DTW, pruned DTW, enroll and identify
├── bonus.cs             # Synchronized DTW (bonus task)
├── Program.cs           # Console menu and entry point
├── GUI/                 # Recorder window (Windows Forms)
├── MFCC/                # MFCC feature extraction and silence removal
├── MainFuctions/        # Audio file loading and feature-extraction helpers
├── Recorder/            # WAV encoder, decoder and recording
├── TestCasesRunner.cs   # Runs test cases and reports accuracy and time
├── TestcaseLoader.cs    # Loads test sets
├── TestController.cs    # Test coordination
├── Recorder.csproj      # Project file
└── Recorder.sln         # Solution file
```

---

## 🚀 Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/IbrahimYasserM/SpeakerID.git
   ```

2. Open `Recorder.sln` in Visual Studio (Windows).
3. Build and run (**F5**). A console window opens with the menu above.
