import { useNavigate } from "react-router-dom"

type HeaderProps = {
    showRegisterForm: React.Dispatch<React.SetStateAction<boolean>>;
    showLoginForm: React.Dispatch<React.SetStateAction<boolean>>;
};

export default function Headbar({ showRegisterForm, showLoginForm }: HeaderProps) {
    const navigate = useNavigate();

    return (
        <div className="fixed top-0 w-full border-b-2 flex flex-row h-12 px-4 md:px-20 justify-between bg-white z-10">
            <div className="flex flex-row gap-4 md:gap-8">
                <button className="text-lg md:text-xl font-bold" onClick={() => navigate("/")}> LingoLift </button>
                <button className="text-sm md:text-lg" onClick={() => navigate("/about")}> About us </button>
                <button className="text-sm md:text-lg" onClick={() => navigate("/contact")}> Contact </button>
            </div>
            <div className="flex flex-row gap-4 md:gap-8 justify-center">
                <button onClick={() => showLoginForm(true)} className="self-center bg-blue-600 rounded-xl h-3/4 w-20 md:w-24 text-white font-bold"> Log in </button>
                <button onClick={() => showRegisterForm(true)} className="self-center bg-blue-600 rounded-xl h-3/4 w-20 md:w-24 text-white font-bold"> Sign up </button>
            </div>
        </div>
    );
}