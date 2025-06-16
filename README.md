# Input Resolution Demo - POC

A proof of concept demonstrating automatic dependency resolution and execution ordering for diagnostic providers in C#.

## Overview

This POC showcases a system that can automatically determine the correct execution order of diagnostic providers based on their input dependencies, similar to dependency injection but for data flow resolution.

## Key Features

- **Automatic Dependency Resolution**: Providers declare their required inputs and what they can resolve
- **Dynamic Execution Ordering**: The system automatically determines the correct order to execute providers
- **Input Propagation**: Results from one provider automatically become available as inputs for subsequent providers
- **Circular Dependency Detection**: Throws exceptions when dependencies cannot be resolved

## Example Providers

- **GetDeviceInfo**: Requires `tcode` → Resolves `imei`
- **CheckICloud**: Requires `imei` → Performs iCloud status check

## How It Works

1. Providers implement `IProvider` interface declaring their input requirements and outputs
2. `ProviderController.ResolveProviderOrder()` analyzes dependencies and creates execution plan
3. Providers execute in resolved order, with each provider's outputs becoming available inputs
4. Results are collected and returned
