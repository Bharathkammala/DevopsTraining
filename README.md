# 🔢 Prime Numbers Generator — DevOps CI Demo

![CI Main](https://github.com/Bharathkammala/DevopsTraining/actions/workflows/dotnet.yml/badge.svg?branch=main)
![CI Dev](https://github.com/Bharathkammala/DevopsTraining/actions/workflows/testing.yml/badge.svg?branch=testing)

A C# .NET Framework console application that generates all prime numbers up to **N** using an optimized square-root algorithm. This repository is also a hands-on demonstration of **GitHub Actions CI/CD** with branch-specific workflow triggers.

---

## 📌 Table of Contents

- [Project Overview](#-project-overview)
- [Architecture](#-architecture)
- [Algorithm Explained](#-algorithm-explained)
- [C# Code Walkthrough](#-c-code-walkthrough)
- [GitHub Actions CI](#-github-actions-ci)
- [How to Run Locally](#-how-to-run-locally)
- [Tech Stack](#-tech-stack)

---

## 🧭 Project Overview

This project serves two purposes:

1. **Algorithm Practice** — Implements an efficient prime number generator using mathematical optimizations (square-root bound + odd-number iteration).
2. **DevOps Practice** — Configures GitHub Actions to automate build pipelines that fire only when code is pushed to specific branches (`main` or `dev`).

---

## 🏗 Architecture

### Repository Structure
```
DevopsTraining/
├── .github/
│   └── workflows/
│       ├── dotnet.yml      ← CI pipeline for the 'main' branch
│       └── testing.yml       ← CI pipeline for the 'testing' branch
├── PrimeNumbersGenerator/
│   ├── Program.cs               ← Core algorithm logic
│   ├── PrimeNumbersGenerator.csproj
│   └── App.config
├── .vs/                         ← Visual Studio metadata (local only)
└── README.md
```

### CI/CD Flow
```
Developer
  │
  └─── git push origin <branch>
              │
              ▼
        GitHub detects push event
              │
              ▼
        Branch filter check
         ┌────┴────┐
       main       dev        feature/* (no trigger)
         │          │
         ▼          ▼
      dotnet      testing
      .yml          .yml
         │          │
         └────┬─────┘
              ▼
     ubuntu-latest runner
     ┌──────────────────┐
     │ 1. Checkout code │
     │ 2. Setup .NET    │
     │ 3. Restore pkgs  │
     │ 4. Build         │
     └──────────────────┘
              │
        ✅ Pass / ❌ Fail
```

---

## ⚙️ Algorithm Explained

The algorithm generates all prime numbers from **2 to N** using three layers of optimization:

### Step 1 — Filter Even Numbers Immediately

> **2** is printed as a special case (the only even prime).  
> The outer loop then starts at **3** and increments by **2**, so the sequence is `3, 5, 7, 9, 11 ...`  
> Even numbers are structurally skipped — **halving the search space instantly.**

### Step 2 — Square-Root Upper Bound

> For any composite number `n`, at least one divisor must be ≤ √n.  
> So instead of checking divisors from `2` to `n-1`, we only check up to **√n**.  
> If no factor is found by then, the number is **guaranteed prime**.

### Step 3 — Iterate Only Over Odd Divisors

> The inner divisor loop also starts at **3** and steps by **2**.  
> Since the candidate is odd, no even number can divide it evenly.  
> This roughly **halves the inner loop work** as well.

### Complexity Comparison

| Metric | Naïve Approach | This Implementation |
|---|---|---|
| Candidates tested | 2 … N (all) | 3, 5, 7 … N (odd only) |
| Divisors per candidate | 2 … n−1 | 3, 5 … √n (odd only) |
| Inner loop iterations | O(n) | O(√n / 2) |
| Overall complexity | O(N²) | O(N · √N / 2) ✅ ~4× faster |

### Pseudocode
```
print 2   // only even prime — handled as special case

for candidate = 3 to N step 2:        // odd candidates only
    isPrime = true

    sqrtCandidate = √candidate

    for divisor = 3 to sqrtCandidate step 2:   // odd divisors only
        if candidate % divisor == 0:
            isPrime = false
            break                      // exit early — composite confirmed

    if isPrime:
        print candidate
```

---


## 🧩 C# Code Walkthrough

### Entry Point
```csharp
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers
{
    internal class Program
    {
        static void Main(string[] args)
        {
```

### Read User Input
```csharp
            Console.Write("Enter number: ");
            int n = int.Parse(Console.ReadLine());
```

### Outer Loop — Every Candidate from 2 to N
```csharp
            for (int i = 2; i <= n; i++)
            {
                bool isPrime = true;
```

### Filter Even Numbers Instantly
```csharp
                if (i > 2 && i % 2 == 0)
                    isPrime = false;
```

### Inner Loop — Trial Division up to √i (Odd Divisors Only)
```csharp
                else
                {
                    for (int j = 3; j * j <= i; j += 2)
                    {
                        if (i % j == 0)
                        {
                            isPrime = false;
                            break;   // composite confirmed — stop checking
                        }
                    }
                }
```

### Print and Close
```csharp
                if (i == 2 || isPrime)
                    Console.Write(i + " ");
            }
        }
    }
}
```


---

## 🔄 GitHub Actions CI

Two separate workflow files are used — one per branch. Code pushed to any other branch (e.g. `feature/*`) triggers **nothing**.

| Branch | Workflow File | Trigger | Build Config |
|---|---|---|---|
| `main` | `dotnet.yml` | push to `main` | Debug + Tests |
| `dev` | `testing.yml` | push to `testing` | Debug + Tests |

### dotnet.yml
```yaml
# Fires ONLY on push to the 'main' branch
name: CI - Main Branch

on:
  push:
    branches:
      - main

jobs:
  build:
    runs-on: windows-latest

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v2
        with:
          dotnet-version: '6.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release
```

### testing.yml
```yaml
# Fires ONLY on push to the 'testing' branch
name: CI - testing Branch

on:
  push:
    branches:
      - testing

jobs:
  build:
    runs-on: windows-latest

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v2
        with:
          dotnet-version: '6.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Debug

      # Dev branch also runs unit tests if present
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

---

## ▶️ How to Run Locally

1. **Clone the repository**
```bash
   git clone https://github.com/Bharathkammala/DevopsTraining.git
   cd DevopsTraining
```

2. **Open in Visual Studio**  
   Double-click `PrimeNumbersGenerator.sln` or use `File → Open → Project/Solution`

3. **Build the solution**  
   Press `Ctrl + Shift + B` or go to `Build → Build Solution`

4. **Run the application**  
   Press `F5` (Debug) or `Ctrl + F5` (Run without debugger)

5. **Enter your number when prompted**
```
   Enter a number N: 50
   2 3 5 7 11 13 17 19 23 29 31 37 41 43 47
```

---

## 🛠 Tech Stack

| Technology | Role |
|---|---|
| C# | Application language |
| .NET Framework | Runtime & SDK |
| Visual Studio | IDE |
| Git & GitHub | Version control & hosting |
| GitHub Actions | CI/CD automation |
| ubuntu-latest | Build runner environment |

---

## 🎯 Purpose

This repository was created to:

- Practice **GitHub Actions CI** setup from scratch
- Demonstrate **branch-specific workflow triggers** (main vs dev)
- Implement and explain an **optimized prime number algorithm**
- Showcase **DevOps fundamentals** in a real project

---

## 👤 Author

**Bharath Kammala** — [github.com/Bharathkammala](https://github.com/Bharathkammala)
