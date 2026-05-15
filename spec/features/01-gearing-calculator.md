# Feature: RC Gearing Calculator

**1. UI & Navigation**
* **Location:** Add a new **"Tools"** dropdown category to the main Navigation Bar.
* **Future-Proofing:** The Gearing Calculator will be the first item, with the menu structured to easily add more tools later.

**2. User Inputs & Pre-selections**
The calculator needs the following inputs to generate the data table:
* **Spur Gear Range:** Input fields for a Minimum and Maximum number of teeth for the spur gear.
* **Pinion Gear Range:** Input fields for a Minimum and Maximum number of teeth for the pinion gear.
* **Internal Gearbox Ratio:**
    * **Manual Input:** A field to type in a custom internal ratio.
    * **Pre-select Dropdown (Combo Box):** A quick-select menu populated with popular chassis to automatically fill the ratio.
    * *Initial Setup Example:* Include the **Tamiya TT02** with an internal ratio of **2.6**.
* **Target Ratio:** A specific final transmission ratio the user wants to hit (e.g., `6.0`).

**3. Core Logic & Calculation**
* **Formula:** The system will calculate the Final Drive Ratio (FDR) using the standard formula:
  **(Spur ÷ Pinion) × Internal Ratio = Final Ratio**
* **Matrix Generation:** The tool will generate a matrix or list of all possible gear combinations based on the Min/Max ranges provided for both the spur and pinion gears.

**4. Visual Output**
* **Target Highlighting:** The system will scan the generated results and automatically highlight the specific gear combination(s) that produce a final ratio closest to the user's requested "Target Ratio."