# 🔢 Prime Numbers Generator — DevOps CI Demo

![CI Main](https://github.com/Bharathkammala/DevopsTraining/actions/workflows/dotnet.yml/badge.svg?branch=main)
![CI Testing](https://github.com/Bharathkammala/DevopsTraining/actions/workflows/testing.yml/badge.svg?branch=testing)
![List Repos](https://github.com/Bharathkammala/DevopsTraining/actions/workflows/list-repos.yml/badge.svg)

A C# .NET Framework console application that generates all prime numbers up to **N** using an optimized square-root algorithm. This repository is also a hands-on demonstration of **GitHub Actions CI/CD** with branch-specific workflow triggers.

---

## 📌 Table of Contents

- [Project Overview](#-project-overview)
- [Architecture](#-architecture)
- [Algorithm Explained](#-algorithm-explained)
- [C# Code Walkthrough](#-c-code-walkthrough)
- [GitHub Actions CI](#-github-actions-ci)
- [List Repositories Workflow](#-github-actions--list-repositories-manual-trigger)
- [How to Run Locally](#-how-to-run-locally)
- [Tech Stack](#-tech-stack)

---

## 🧭 Project Overview

This project serves two purposes:

1. **Algorithm Practice** — Implements an efficient prime number generator using mathematical optimizations (square-root bound + odd-number iteration).
2. **DevOps Practice** — Configures GitHub Actions to automate build pipelines that fire only when code is pushed to specific branches (`main` or `testing`), plus a manual workflow to fetch GitHub repository data via the GitHub API.

---

## 🏗 Architecture

### Repository Structure
```
DevopsTraining/
├── .github/
│   └── workflows/
│       ├── dotnet.yml           ← CI pipeline for the 'main' branch
│       ├── testing.yml          ← CI pipeline for the 'testing' branch
│       └── list-repos.yml       ← Manual workflow to list GitHub repos
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
       main     testing      feature/* (no trigger)
         │          │
         ▼          ▼
      dotnet     testing
      .yml         .yml
         │          │
         └────┬─────┘
              ▼
     windows-latest runner
     ┌──────────────────┐
     │ 1. Checkout code │
     │ 2. Setup .NET    │
     │ 3. Restore pkgs  │
     │ 4. Build         │
     └──────────────────┘
              │
        ✅ Pass / ❌ Fail


Manual Trigger (workflow_dispatch)
  │
  └─── GitHub Actions Tab → Run workflow
              │
              ▼
        list-repos.yml
              │
              ▼
     windows-latest runner
     ┌──────────────────────────┐
     │ 1. Checkout code         │
     │ 2. Call GitHub REST API  │
     │ 3. Print repo names      │
     └──────────────────────────┘
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
| `main` | `dotnet.yml` | push to `main` | Release |
| `testing` | `testing.yml` | push to `testing` | Debug + Tests |

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
name: CI - Testing Branch

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

      # Testing branch also runs unit tests if present
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

---

## 📋 GitHub Actions — List Repositories (Manual Trigger)

This workflow fetches and displays all public repositories of a given GitHub user using the **GitHub REST API** and **PowerShell**. Unlike the CI workflows, this one is triggered **manually** — not on a code push.

### ⚡ Trigger Type — `workflow_dispatch`
```yaml
on:
  workflow_dispatch:
    inputs:
      username:
        description: "GitHub username"
        required: true
        default: "Bharathkammala"
```

`workflow_dispatch` means this workflow only runs when you **manually trigger it** from the GitHub Actions tab. It accepts a username input at runtime, with `Bharathkammala` as the default value.

### list-repos.yml
```yaml
name: List GitHub Repositories

on:
  workflow_dispatch:
    inputs:
      username:
        description: "GitHub username"
        required: true
        default: "Bharathkammala"

jobs:
  list-repos:
    runs-on: windows-latest
    env:
      GITHUB_USERNAME: ${{ github.event.inputs.username }}

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Display repositories using GitHub API
        shell: pwsh
        run: |
          Write-Host "Fetching repositories for $env:GITHUB_USERNAME"
          $url = "https://api.github.com/users/$env:GITHUB_USERNAME/repos"
          $repos = Invoke-RestMethod -Uri $url
          $repos | ForEach-Object { $_.name }
```

### 🧩 Steps Explained

| Step | What it does |
|---|---|
| `Checkout repository` | Checks out source code onto the runner |
| `Write-Host` | Prints a status message to the Actions log |
| `$url = ...` | Builds the GitHub REST API endpoint for the given username |
| `Invoke-RestMethod` | Makes an HTTP GET request and auto-parses the JSON response |
| `ForEach-Object { $_.name }` | Loops through each repo object and prints its name |

### 🔄 How to Run This Workflow

1. Go to your repository on GitHub
2. Click the **Actions** tab
3. Select **"List GitHub Repositories"** from the left sidebar
4. Click **"Run workflow"**
5. Enter a GitHub username (default: `Bharathkammala`)
6. Click the green **"Run workflow"** button
7. Open the run to see all public repositories listed in the logs

---

### 📊 All Workflows at a Glance

| Feature | dotnet.yml | testing.yml | list-repos.yml |
|---|---|---|---|
| Trigger | Push to `main` | Push to `testing` | Manual only |
| Runner | windows-latest | windows-latest | windows-latest |
| Shell | PowerShell | PowerShell | PowerShell (pwsh) |
| Purpose | Build & validate | Build & test | Fetch GitHub API data |
| User input | None | None | GitHub username |
| Runs automatically | ✅ Yes | ✅ Yes | ❌ No |

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
| windows-latest | Build runner environment |
| PowerShell | Scripting in workflows |
| GitHub REST API | Fetch repository metadata |

---

## 🎯 Purpose

This repository was created to:

- Practice **GitHub Actions CI** setup from scratch
- Demonstrate **branch-specific workflow triggers** (main vs testing)
- Implement and explain an **optimized prime number algorithm**
- Use **manual workflow dispatch** with runtime user inputs
- Call the **GitHub REST API** from within a workflow using PowerShell
- Showcase **DevOps fundamentals** in a real project

---

## 👤 Author

**Bharath Kammala** — [github.com/Bharathkammala](https://github.com/Bharathkammala)
