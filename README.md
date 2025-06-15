# 🩺 MedConnect - Doctor Appointments Reservation System
📺 **Project Demo:** [https://drive.google.com/file/d/13w6EqF-7wQHzqj85vu1DIrrqrg7oVSo9/view?usp=sharing](https://drive.google.com/file/d/13w6EqF-7wQHzqj85vu1DIrrqrg7oVSo9/view?usp=sharing)

**MedConnect** is a full-stack web application built using **ASP.NET MVC** that allows **doctors** and **patients** to manage appointments in a smart, efficient, and reliable way. Inspired by platforms like **Vezeeta**, MedConnect supports **user authentication, role-based access, real-time email notifications**, and **payment integration** for seamless doctor-patient interaction.

---

## 📌 Table of Contents

- [Features](#-features)
- [Technologies Used](#-technologies-used)
- [Roles & Functionalities](#-roles--functionalities)
  - [Admin](#admin)
  - [Doctor](#doctor)
  - [Patient](#patient)
- [Design Patterns](#-design-patterns)
- [Setup & Installation](#-setup--installation)
- [License](#-license)

---

## 🚀 Features

- 🧠 Identity system with role-based access (Admin, Doctor, Patient)
- 📧 Email confirmation and password reset using **SendGrid**
- 🔐 External login via **Google**
- 🗓 Appointment scheduling and management
- 💳 Secure payment system integration using **Stripe**
- 📊 Admin dashboard with detailed statistics and controls
- 🏥 Medical records and history tracking
- ⭐ Reviews and rating system with moderation
- 📱 Responsive and user-friendly interface

---

## 💻 Technologies Used

- ASP.NET Core MVC (.NET 6+)
- Entity Framework Core
- Microsoft Identity
- SendGrid Email API
- Stripe Payment Gateway
- Google OAuth Authentication
- Bootstrap & jQuery (for responsive UI)
- SQL Server

---

## 👥 Roles & Functionalities

### 👨‍💼 Admin

- Full access to system data and analytics:
  - View system statistics: average patient age, gender distribution, doctor ratings, earnings, etc.
- Manage Doctors:
  - Approve trusted doctors with verified documentation
  - CRUD operations on doctors, clinics, and availability schedules
- Manage Patients:
  - Approve or ban patients
  - Moderate reviews (filter hate speech)
  - Access full medical records and appointment history

---

### 🩺 Doctor

- Register, create profile with image and personal details
- Await approval from Admin before becoming active
- Manage clinics and availability:
  - Set working hours by day, location (online/offline)
- View and manage appointments:
  - Reschedule/cancel appointments and notify patients
- Access patient records and write medical reports

---

### 👨‍⚕️ Patient

- Register and log in using email or Google account
- Browse/filter doctors by:
  - Specialization, Title, Fees, Reviews, Availability
- Book appointments:
  - Select slot and clinic (online or offline)
  - Pay using Visa or choose cash payment (via Stripe)
- View upcoming and past appointments
- Add reviews and rate doctors
- Cancel bookings and receive automated refund notifications

---

## 📐 Design Patterns

The following design patterns were used:

- **Dependency Injection** - for managing services and dependencies.
- **Repository Pattern** - for clean and maintainable data access logic.
- **Observer Pattern** - to notify users (patients/doctors) about important events such as:
  - Appointment booked/rescheduled/cancelled
  - Payment success/failure
  - Admin approval status


