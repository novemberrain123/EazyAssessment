import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { apiFetch } from "../services/api";

function Login() {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    async function handleSubmit(e: React.SubmitEvent) {
        e.preventDefault();

        setError("");

        try {
            const result = await apiFetch("/api/auth/login", {
                method: "POST",
                body: JSON.stringify({
                    email,
                    password,
                }),
            });

            localStorage.setItem("token", result.token);

            navigate("/profile");
        } catch (err) {
            if (err instanceof Error) {
                setError(err.message);
            } else {
                setError("An unexpected error occurred.");
            }
        }     }

    return (
        <div style={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            justifyContent: "center",
            height: "50vh",
        }}>

            <form
                style={{
                    display: "grid",
                    gridTemplateColumns: "120px 1fr",
                    gap: "12px",
                    width: "400px",
                }}
                onSubmit={handleSubmit}>
                <label>Email</label>
                <input
                    maxLength={100}
                    type="email"
                    autoComplete="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />

                <label>Password</label>
                <input
                    type="password"
                    autoComplete="current-password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                <div></div>

                <span className="invalid-feedback small">{error}</span>

                <div></div>

                <button style={{ width: "50%" }} type="submit" >
                Login
                </button>
            </form>
        </div>
    );
}

export default Login;