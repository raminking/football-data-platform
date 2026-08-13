# Lessons Learned

This document records important technical lessons learned during development.

The purpose is not to document every line of code.

The purpose is to record decisions and concepts that are important enough to remember later.

---

## 2026-08-12

### Architecture — Vertical Slice Architecture

Today I learned why Vertical Slice Architecture can be preferable to traditional layered architecture for feature-oriented applications.

Traditional layered architecture commonly organizes code around technical concerns:

```text
Controllers
Services
Repositories
Entities